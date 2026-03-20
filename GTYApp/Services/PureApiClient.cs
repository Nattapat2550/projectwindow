using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GTYApp.Utils;

namespace GTYApp.Services
{
    public static class PureApiClient
    {
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        // ตั้งค่า fallback เป็น localhost (เพื่อรองรับการรัน Backend เองในเครื่อง)
        private static string _baseUrl = "http://localhost:5000/api";
        // ตัวแปรสำหรับเก็บ API Key
        private static string _apiKey = "";

        public static void ConfigureFromAppSettings()
        {
            // อ่าน BaseUrl จาก appsettings.json
            var url = AppConfig.GetString("ApiPure", "BaseUrl");
            if (!string.IsNullOrWhiteSpace(url))
            {
                _baseUrl = url.TrimEnd('/');
            }

            // อ่าน ApiKey จาก appsettings.json
            var apiKey = AppConfig.GetString("ApiPure", "ApiKey");
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _apiKey = apiKey;
            }
        }

        public static JsonElement Send(HttpMethod method, string path, object? body = null, bool withJwt = false)
        {
            return SendAsync(method, path, body, withJwt).GetAwaiter().GetResult();
        }

        public static async Task<JsonElement> SendAsync(HttpMethod method, string path, object? body = null, bool withJwt = false)
        {
            // ตรวจสอบเพื่อไม่ให้ลืมใส่เครื่องหมาย / นำหน้า path 
            if (!path.StartsWith("/"))
            {
                path = "/" + path;
            }

            var req = new HttpRequestMessage(method, _baseUrl + path);

            // ⭐ แนบ API Key เข้าไปใน Header ทุกครั้งเพื่อแก้ปัญหา 401 Unauthorized
            if (!string.IsNullOrWhiteSpace(_apiKey))
            {
                req.Headers.Add("x-api-key", _apiKey);
            }

            if (withJwt && !string.IsNullOrWhiteSpace(Session.Jwt))
            {
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Session.Jwt);
            }

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body);
                req.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            try
            {
                using var resp = await _http.SendAsync(req);
                var respStr = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    string errorMsg = $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}";
                    try
                    {
                        using var errorDoc = JsonDocument.Parse(respStr);
                        if (errorDoc.RootElement.TryGetProperty("message", out var msgProp))
                        {
                            errorMsg = msgProp.GetString() ?? errorMsg;
                        }
                        else if (errorDoc.RootElement.TryGetProperty("error", out var errProp))
                        {
                            errorMsg = errProp.GetString() ?? errorMsg;
                        }
                    }
                    catch
                    {
                        // ถ้าไม่ได้ส่งกลับมาเป็น JSON ก็ใช้ HTTP Error หลักแทน
                    }
                    throw new Exception(errorMsg);
                }

                if (string.IsNullOrWhiteSpace(respStr))
                {
                    return default;
                }

                using var doc = JsonDocument.Parse(respStr);
                return doc.RootElement.Clone();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("ไม่สามารถเชื่อมต่อกับเซิร์ฟเวอร์ได้ โปรดตรวจสอบอินเทอร์เน็ตหรือสถานะของเซิร์ฟเวอร์", ex);
            }
            catch (TaskCanceledException)
            {
                throw new Exception("การเชื่อมต่อหมดเวลา (Timeout) โปรดลองใหม่อีกครั้ง");
            }
        }
    }
}