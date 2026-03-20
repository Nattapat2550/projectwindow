using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GTYApp.Utils
{
    public static class CryptoStorage
    {
        private static readonly string FilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GTYApp",
            "token.dat"
        );

        public static void SaveRememberToken(string token)
        {
            try
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (dir != null && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var rawBytes = Encoding.UTF8.GetBytes(token);
                var encrypted = ProtectedData.Protect(rawBytes, null, DataProtectionScope.CurrentUser);

                File.WriteAllBytes(FilePath, encrypted);
            }
            catch (Exception)
            {
                // ปล่อยผ่าน
            }
        }

        public static string? TryRestoreRememberToken()
        {
            try
            {
                if (!File.Exists(FilePath)) return null;

                var encrypted = File.ReadAllBytes(FilePath);
                var rawBytes = ProtectedData.Unprotect(encrypted, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(rawBytes);
            }
            catch
            {
                DeleteRememberToken();
                return null;
            }
        }

        // ⭐ เพิ่มกลับมาเพื่อรองรับคำสั่งตอน Logout ในโค้ดเก่า
        public static void Clear()
        {
            DeleteRememberToken();
        }

        public static void DeleteRememberToken()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    File.Delete(FilePath);
                }
            }
            catch
            {
                // ปล่อยผ่าน
            }
        }
    }
}