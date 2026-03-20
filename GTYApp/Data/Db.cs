using Npgsql;
using System;

namespace GTYApp.Data
{
    public static class Db
    {
        private static string? _cs;
        public static string ConnectionString => _cs ??= BuildConnectionString();

        private static string BuildConnectionString()
        {
            var raw = Utils.AppConfig.GetString("Database", "Url");
            if (string.IsNullOrWhiteSpace(raw))
                throw new ArgumentException("Database.Url missing in appsettings.json");

            if (raw.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
                raw.Contains("Server=", StringComparison.OrdinalIgnoreCase))
                return raw;

            if (raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(raw);

                var userInfo = uri.UserInfo.Split(':', 2, StringSplitOptions.None);
                var username = Uri.UnescapeDataString(userInfo[0]);
                var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";

                var database = uri.AbsolutePath.TrimStart('/');

                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.IsDefaultPort ? 5432 : uri.Port,
                    Username = username,
                    Password = password,
                    Database = database,
                    SslMode = SslMode.Require,
                    Timeout = 15,
                    CommandTimeout = 30
                };

                return builder.ToString();
            }

            throw new ArgumentException("Invalid Database.Url format in appsettings.json. Expected key=value pairs or a postgresql:// URL.");
        }

        public static NpgsqlConnection GetOpenConnection()
        {
            try
            {
                var conn = new NpgsqlConnection(ConnectionString);
                conn.Open();
                return conn;
            }
            catch (Exception ex)
            {
                throw new Exception($"ไม่สามารถเชื่อมต่อฐานข้อมูลได้: {ex.Message}", ex);
            }
        }

        public static void Initialize()
        {
            using var conn = GetOpenConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT 1";
            cmd.ExecuteScalar();
        }
    }
}