using GTYApp.Utils;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using GTYApp.Models;

namespace GTYApp.Services
{
    public static class UserService
    {
        public static User? GetMe()
        {
            try
            {
                // ⭐ ใช้ Task.Run ดึง SendAsync ป้องกันอาการ Deadlock บน WinForms UI
                var data = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Get, "/users/me", null, true)).GetAwaiter().GetResult();
                return ParseUserMe(data);
            }
            catch
            {
                return null;
            }
        }

        public static bool UpdateUsername(int _, string newUsername)
        {
            return PatchMe(username: newUsername, profilePictureUrl: null);
        }

        public static bool UpdateProfilePicture(int _, string dataUrl)
        {
            return PatchMe(username: null, profilePictureUrl: dataUrl);
        }

        public static bool DeleteAccount(int id)
        {
            try
            {
                _ = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Post, "/internal/delete-user", new { id }, true)).GetAwaiter().GetResult();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static List<User> GetAllUsers()
        {
            var list = new List<User>();
            if (!Session.IsAdmin) return list;

            try
            {
                // ⭐ ใส่ true ด้านหลังสุด เพื่อแนบ JWT Token (ถ้าไม่แนบ API จะคืนค่า 401 และทำให้เกิด Error)
                var data = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Get, "/internal/admin/users", null, true)).GetAwaiter().GetResult();

                // ⭐ แกะเปลือก "data" ที่ได้จาก Rust Backend เสมอ
                if (data.TryGetProperty("data", out var d)) data = d;

                if (data.ValueKind != JsonValueKind.Array) return list;

                foreach (var u in data.EnumerateArray())
                {
                    list.Add(new User
                    {
                        Id = u.GetProperty("id").GetInt32(),
                        Email = u.GetProperty("email").GetString() ?? "",
                        Username = u.TryGetProperty("username", out var un) && un.ValueKind != JsonValueKind.Null ? un.GetString() : null,
                        Role = u.TryGetProperty("role", out var role) && role.ValueKind != JsonValueKind.Null ? (role.GetString() ?? "user") : "user",
                        IsEmailVerified = u.TryGetProperty("is_email_verified", out var v) && v.ValueKind == JsonValueKind.True,
                        OauthProvider = u.TryGetProperty("oauth_provider", out var pr) && pr.ValueKind != JsonValueKind.Null ? pr.GetString() : null,
                        ProfilePictureUrl = u.TryGetProperty("profile_picture_url", out var pp) && pp.ValueKind != JsonValueKind.Null ? pp.GetString() : null,
                        OauthId = u.TryGetProperty("oauth_id", out var oid) && oid.ValueKind != JsonValueKind.Null ? oid.GetString() : null,
                    });
                }
            }
            catch { }

            return list;
        }

        public static bool UpdateUser(User u)
        {
            if (!Session.IsAdmin) return false;

            try
            {
                _ = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Patch, $"/users/{u.Id}/role", new { role = u.Role }, true)).GetAwaiter().GetResult();

                _ = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Post, "/internal/admin/users/update", new
                {
                    id = u.Id,
                    username = u.Username,
                    profile_picture_url = u.ProfilePictureUrl
                }, true)).GetAwaiter().GetResult();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static bool PatchMe(string? username, string? profilePictureUrl)
        {
            try
            {
                _ = Task.Run(() => PureApiClient.SendAsync(HttpMethod.Patch, "/users/me", new
                {
                    username,
                    profile_picture_url = profilePictureUrl
                }, true)).GetAwaiter().GetResult();

                var me = GetMe();
                if (me is not null)
                    Session.CurrentUser = me;

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static User ParseUserMe(JsonElement me)
        {
            if (me.TryGetProperty("data", out var d) && d.ValueKind == JsonValueKind.Object) me = d;
            else if (me.TryGetProperty("user", out var u) && u.ValueKind == JsonValueKind.Object) me = u;

            return new User
            {
                Id = me.GetProperty("id").GetInt32(),
                Username = me.TryGetProperty("username", out var un) && un.ValueKind != JsonValueKind.Null ? un.GetString() : null,
                Email = me.GetProperty("email").GetString() ?? "",
                Role = me.TryGetProperty("role", out var role) && role.ValueKind != JsonValueKind.Null ? (role.GetString() ?? "user") : "user",
                ProfilePictureUrl = me.TryGetProperty("profile_picture_url", out var pp) && pp.ValueKind != JsonValueKind.Null ? pp.GetString() : null,
                IsEmailVerified = me.TryGetProperty("is_email_verified", out var v) && v.ValueKind == JsonValueKind.True
            };
        }
    }
}