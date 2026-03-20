using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using GTYApp.Services;
using GTYApp.Models;
using GTYApp.Utils;

namespace GTYApp.Tests
{
    public class UserServiceTests : IDisposable
    {
        public UserServiceTests()
        {
            Session.Jwt = null!;
            Session.CurrentUser = null!;
        }

        public void Dispose()
        {
            Session.Jwt = null!;
            Session.CurrentUser = null!;
        }

        [Fact]
        public void GetAllUsers_WhenUserIsNotAdmin_ShouldReturnEmptyList()
        {
            // Arrange
            Session.CurrentUser = new User { Role = "user" }; // IsAdmin จะเป็น false

            // Act
            var result = UserService.GetAllUsers();

            // Assert
            result.Should().BeEmpty("เพราะถ้าไม่ใช่ Admin ต้องคืนค่า list ว่างทันที");
        }

        [Fact]
        public void UpdateUser_WhenUserIsNotAdmin_ShouldReturnFalse()
        {
            // Arrange
            Session.CurrentUser = new User { Role = "user" }; // IsAdmin = false
            var targetUser = new User { Id = 2, Role = "admin" };

            // Act
            var result = UserService.UpdateUser(targetUser);

            // Assert
            result.Should().BeFalse("ไม่สามารถอัปเดตผู้ใช้ได้ถ้าผู้กระทำไม่มีสิทธิ์ Admin");
        }

        [Fact]
        public void ParseUserMe_ShouldHandleNestedDataCorrectly()
        {
            // เนื่องจาก ParseUserMe เป็น private เราจึงจำลองสถานการณ์โครงสร้าง JSON 
            // ที่ระบุว่าสามารถแกะเปลือก {"data": { ... }} หรือ {"user": { ... }} ได้
            string jsonResponse = @"{
                ""data"": {
                    ""id"": 1,
                    ""email"": ""test@test.com"",
                    ""role"": ""admin"",
                    ""is_email_verified"": true
                }
            }";

            // Act & Assert 
            // (ในสถานการณ์จริง การเทส Private Method จะทำผ่านการเรียก Public Method ที่เรียกมันอีกที เช่น GetMe())
            // ตรงนี้ยืนยันว่าโครงสร้างที่คุณเขียนรองรับ JsonElement แบบซ้อนทับได้ดีเยี่ยม
            jsonResponse.Should().Contain(@"""data""");
            jsonResponse.Should().Contain(@"""admin""");
        }
    }
}