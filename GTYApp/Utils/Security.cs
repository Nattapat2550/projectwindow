using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace GTYApp.Utils
{
    public static partial class Security
    {
        // --- รองรับเดิม (SHA-256) ---
        public static string HashPasswordSha256(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            var sb = new StringBuilder();
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        // --- ใช้ bcrypt เป็นค่า default สำหรับ hash ใหม่ ---
        public static string HashPassword(string password, int workFactor = 12)
            => BCrypt.Net.BCrypt.HashPassword(password, workFactor);

        [GeneratedRegex("^[0-9a-f]{64}$", RegexOptions.Compiled)]
        private static partial Regex Sha256HexRegex();

        public static bool VerifyPassword(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(storedHash))
                return false;

            // ถ้าเป็น bcrypt ($2a/$2b/$2y)
            if (storedHash.StartsWith("$2a$") || storedHash.StartsWith("$2b$") || storedHash.StartsWith("$2y$"))
                return BCrypt.Net.BCrypt.Verify(password, storedHash);

            // ถ้าเป็น SHA-256 hex 64 ตัว
            if (storedHash.Length == 64 && Sha256HexRegex().IsMatch(storedHash))
                return HashPasswordSha256(password) == storedHash;

            // รูปแบบไม่รู้จัก: ปฏิเสธ
            return false;
        }
    }
}
