using GTYApp.Utils;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GTYApp.Models;

namespace GTYApp.Services
{
    public static class AuthService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(20) };

        public static string? LastError { get; private set; }

        public static async Task<bool> TryResumeSessionAsync(string token)
        {
            try
            {
                LastError = null;
                if (string.IsNullOrWhiteSpace(token)) return false;
                Session.Jwt = token;

                var me = await PureApiClient.SendAsync(HttpMethod.Get, "/auth/me", withJwt: true);
                Session.CurrentUser = ParseUser(me);
                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                Session.SignOut();
                return false;
            }
        }

        public static async Task<bool> LoginWithPasswordAsync(string email, string password, bool remember)
        {
            try
            {
                LastError = null;
                var data = await PureApiClient.SendAsync(HttpMethod.Post, "/auth/login", new
                {
                    email = (email ?? "").Trim(),
                    password = password ?? ""
                });

                var (jwt, user) = ParseAuthResponse(data);

                Session.Jwt = jwt;
                Session.CurrentUser = user;

                if (remember) CryptoStorage.SaveRememberToken(jwt);

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public static async Task<bool> LoginWithGoogleLoopbackAsync()
        {
            try
            {
                LastError = null;

                var clientId = AppConfig.GetString("OAuth", "Google", "ClientId");
                var clientSecret = AppConfig.GetString("OAuth", "Google", "ClientSecret");
                var redirectUri = AppConfig.GetString("OAuth", "Google", "RedirectUri");
                var scopes = AppConfig.GetString("OAuth", "Google", "Scopes") ?? "openid email profile";

                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret) || string.IsNullOrWhiteSpace(redirectUri))
                    throw new Exception("Google OAuth config missing (ClientId/ClientSecret/RedirectUri)");

                var ru = new Uri(redirectUri!);
                var state = Guid.NewGuid().ToString("N");

                var authorizeUrl =
                    "https://accounts.google.com/o/oauth2/v2/auth" +
                    $"?client_id={Uri.EscapeDataString(clientId!)}" +
                    $"&redirect_uri={Uri.EscapeDataString(redirectUri!)}" +
                    $"&response_type=code" +
                    $"&scope={Uri.EscapeDataString(scopes)}" +
                    $"&state={Uri.EscapeDataString(state)}" +
                    $"&access_type=offline" +
                    $"&prompt=consent";

                using var listener = new HttpListener();
                var prefix = $"{ru.Scheme}://{ru.Host}:{ru.Port}/";
                listener.Prefixes.Add(prefix);

                try
                {
                    listener.Start();
                }
                catch (HttpListenerException)
                {
                    throw new Exception($"ไม่สามารถเปิดใช้งาน Port ({ru.Port}) สำหรับ Google Login ได้ (พอร์ตอาจถูกโปรแกรมอื่นใช้งานอยู่)");
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = authorizeUrl,
                    UseShellExecute = true
                });

                HttpListenerContext ctx = null!;
                var timeoutTask = Task.Delay(TimeSpan.FromMinutes(3));

                while (true)
                {
                    var getContextTask = listener.GetContextAsync();
                    var completedTask = await Task.WhenAny(getContextTask, timeoutTask);

                    if (completedTask == timeoutTask)
                    {
                        listener.Stop();
                        throw new Exception("หมดเวลาการล็อกอินผ่านเบราว์เซอร์ (Timeout)");
                    }

                    var tempCtx = await getContextTask;

                    if (string.Equals(tempCtx.Request.Url?.AbsolutePath, ru.AbsolutePath, StringComparison.OrdinalIgnoreCase))
                    {
                        ctx = tempCtx;
                        break;
                    }
                    else
                    {
                        tempCtx.Response.StatusCode = 404;
                        tempCtx.Response.OutputStream.Close();
                    }
                }

                var req = ctx.Request;
                var res = ctx.Response;

                var code = req.QueryString["code"];
                var st = req.QueryString["state"];

                var htmlOk = "<html><body><h3>Login success. You can close this window.</h3><script>window.close();</script></body></html>";
                var buf = Encoding.UTF8.GetBytes(htmlOk);
                res.ContentType = "text/html";
                res.ContentLength64 = buf.Length;
                res.OutputStream.Write(buf, 0, buf.Length);
                res.OutputStream.Close();

                if (string.IsNullOrWhiteSpace(code) || st != state)
                    throw new Exception("Google auth code missing or state mismatch");

                var tokenJson = await ExchangeGoogleTokenAsync(code!, clientId!, clientSecret!, redirectUri!);
                using var tokenDoc = JsonDocument.Parse(tokenJson);

                if (!tokenDoc.RootElement.TryGetProperty("id_token", out var idTokenProp) || string.IsNullOrWhiteSpace(idTokenProp.GetString()))
                    throw new Exception("Missing id_token from Google");

                var idToken = idTokenProp.GetString();
                var payload = ParseJwtPayload(idToken!);

                var email = payload.TryGetProperty("email", out var e) ? e.GetString() : null;
                var sub = payload.TryGetProperty("sub", out var s) ? s.GetString() : null;
                var name = payload.TryGetProperty("name", out var n) ? n.GetString() : null;
                var picture = payload.TryGetProperty("picture", out var p) ? p.GetString() : null;

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(sub))
                    throw new Exception("Google token payload missing email or sub");

                var data = await PureApiClient.SendAsync(HttpMethod.Post, "/auth/oauth/google", new
                {
                    email = email!,
                    oauthId = sub!,
                    username = name,
                    pictureUrl = picture
                });

                var (jwt, user) = ParseAuthResponse(data);
                Session.Jwt = jwt;
                Session.CurrentUser = user;

                return true;
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                return false;
            }
        }

        public static async Task LogoutAsync()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Session.Jwt))
                    await PureApiClient.SendAsync(HttpMethod.Post, "/auth/logout", null, withJwt: true);
            }
            catch { }
            finally
            {
                Session.SignOut();
            }
        }

        private static (string jwt, User user) ParseAuthResponse(JsonElement data)
        {
            // ⭐ ตรวจจับและแกะเปลือก { "ok": true, "data": { ... } } จากระบบ Rust Backend
            if (data.TryGetProperty("data", out var dataWrapper) && dataWrapper.ValueKind == JsonValueKind.Object)
            {
                data = dataWrapper;
            }

            var jwt = data.TryGetProperty("token", out var t) ? t.GetString() ?? "" : "";
            if (string.IsNullOrWhiteSpace(jwt)) throw new Exception("Missing token in server response");

            if (!data.TryGetProperty("user", out var u))
                throw new Exception("Missing user data in server response");

            return (jwt, ParseUser(u));
        }

        private static User ParseUser(JsonElement u)
        {
            // ⭐ ตรวจจับและแกะเปลือกสำหรับ API บางตัวที่คืน Object ซ้อน
            if (u.TryGetProperty("data", out var dataWrapper) && dataWrapper.ValueKind == JsonValueKind.Object)
            {
                u = dataWrapper;
            }
            else if (u.TryGetProperty("user", out var userWrapper) && userWrapper.ValueKind == JsonValueKind.Object)
            {
                u = userWrapper;
            }

            return new User
            {
                Id = u.TryGetProperty("id", out var idProp) && idProp.ValueKind == JsonValueKind.Number ? idProp.GetInt32() : 0,
                Email = u.TryGetProperty("email", out var emailProp) ? (emailProp.GetString() ?? "") : "",
                Username = u.TryGetProperty("username", out var un) && un.ValueKind != JsonValueKind.Null ? un.GetString() : null,
                Role = u.TryGetProperty("role", out var r) && r.ValueKind != JsonValueKind.Null ? (r.GetString() ?? "user") : "user",
                ProfilePictureUrl = u.TryGetProperty("profile_picture_url", out var pp) && pp.ValueKind != JsonValueKind.Null ? pp.GetString() : null,
                IsEmailVerified = u.TryGetProperty("is_email_verified", out var v) && v.ValueKind == JsonValueKind.True,
                OauthProvider = u.TryGetProperty("oauth_provider", out var op) && op.ValueKind != JsonValueKind.Null ? op.GetString() : "local",
                OauthId = null
            };
        }

        private static async Task<string> ExchangeGoogleTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            var body =
                $"code={Uri.EscapeDataString(code)}" +
                $"&client_id={Uri.EscapeDataString(clientId)}" +
                $"&client_secret={Uri.EscapeDataString(clientSecret)}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                "&grant_type=authorization_code";

            using var content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            var resp = await _http.PostAsync("https://oauth2.googleapis.com/token", content);
            resp.EnsureSuccessStatusCode();
            return await resp.Content.ReadAsStringAsync();
        }

        private static JsonElement ParseJwtPayload(string jwt)
        {
            var parts = jwt.Split('.');
            if (parts.Length < 2) throw new Exception("Invalid JWT Structure");

            var payload = parts[1].Replace('-', '+').Replace('_', '/');

            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var bytes = Convert.FromBase64String(payload);
            using var doc = JsonDocument.Parse(bytes);
            return doc.RootElement.Clone();
        }
    }
}