using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using GTYApp.Services;
using GTYApp.Models;
using GTYApp.Utils;

namespace GTYApp.Tests
{
    // ใช้ IDisposable เพื่อรีเซ็ต State ของ Session ทุกครั้งก่อน/หลังรันแต่ละเทส
    public class AuthServiceTests : IDisposable
    {
        public AuthServiceTests()
        {
            // Setup: ล้าง Session ก่อนเริ่มเทสทุกครั้งป้องกัน State รั่วไหล
            Session.Jwt = null;
            Session.CurrentUser = null;
        }

        public void Dispose()
        {
            // Teardown: ล้าง Session ทิ้งหลังเทสเสร็จ
            Session.Jwt = null;
            Session.CurrentUser = null;
        }

        [Fact]
        public async Task TryResumeSessionAsync_WithEmptyToken_ShouldReturnFalse()
        {
            // Act
            var result = await AuthService.TryResumeSessionAsync("");

            // Assert
            result.Should().BeFalse();
            Session.Jwt.Should().BeNullOrWhiteSpace();
            Session.CurrentUser.Should().BeNull();
        }

        [Fact]
        public async Task TryResumeSessionAsync_WithNullToken_ShouldReturnFalse()
        {
            // Act
            var result = await AuthService.TryResumeSessionAsync(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task LoginWithPasswordAsync_WithEmptyCredentials_ShouldHandleSafely()
        {
            // Act
            // เทสว่าถ้าส่ง null เข้าไป โค้ดที่ใช้ (email ?? "").Trim() จะไม่พัง (No NullReferenceException)
            var result = await AuthService.LoginWithPasswordAsync(null!, null!, false);

            // Assert
            // เนื่องจากไม่ได้เชื่อมต่อ API จริงใน Test มันควรจะ Throw Exception แจ้งเตือนการเชื่อมต่อ
            // และคืนค่า False พร้อมบันทึกข้อผิดพลาดใน LastError
            result.Should().BeFalse();
            AuthService.LastError.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LogoutAsync_ShouldClearSession()
        {
            // Arrange
            Session.Jwt = "fake-jwt-token";
            Session.CurrentUser = new User { Id = 1, Username = "TestUser" };

            // Act
            await AuthService.LogoutAsync();

            // Assert
            Session.Jwt.Should().BeNullOrWhiteSpace("เพราะ SignOut() ต้องถูกเรียกเสมอใน finally");
            Session.CurrentUser.Should().BeNull();
        }
    }
}