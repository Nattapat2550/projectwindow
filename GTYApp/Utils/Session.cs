using System;
using System.Collections.Generic;
using System.Text;
using GTYApp.Models;


namespace GTYApp.Utils
{
    public static class Session
    {
        public static User? CurrentUser { get; set; }
        public static string? Jwt { get; set; }

        public static bool IsAuthenticated => CurrentUser is not null && !string.IsNullOrWhiteSpace(Jwt);

        public static bool IsAdmin =>
            CurrentUser is not null &&
            string.Equals(CurrentUser.Role, "admin", StringComparison.OrdinalIgnoreCase);

        public static void SignOut()
        {
            CurrentUser = null;
            Jwt = null;
            CryptoStorage.Clear();
        }
    }
}