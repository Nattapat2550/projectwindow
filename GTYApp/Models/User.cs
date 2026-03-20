using System;
using System.Collections.Generic;
using System.Text;

namespace GTYApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string Email { get; set; } = "";
        public string? PasswordHash { get; set; }
        public string Role { get; set; } = "user";
        public string? ProfilePictureUrl { get; set; }
        public bool IsEmailVerified { get; set; }
        public string? OauthProvider { get; set; }
        public string? OauthId { get; set; }
    }
}
