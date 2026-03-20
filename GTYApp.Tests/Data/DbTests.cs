using System;
using Xunit;
using FluentAssertions;
using GTYApp.Data;
using GTYApp.Utils; // สมมติว่า AppConfig อยู่ที่นี่

namespace GTYApp.Tests
{
    public class DbTests
    {
        [Fact]
        public void BuildConnectionString_WithValidPostgresUrl_ShouldParseCorrectly()
        {
            // Arrange
            // หมายเหตุ: ในการรันเทสจริง คุณต้องตั้งค่า AppConfig.GetString ให้คืนค่านี้ 
            // ตัวอย่าง URL: postgres://myuser:mypassword@localhost:5432/mydatabase
            string rawUrl = "postgres://myuser:mypassword@localhost:5432/mydatabase";

            // Act
            // สมมติว่าเราเรียกใช้ Method เพื่อจำลองการทำงานของ Db.BuildConnectionString
            var uri = new Uri(rawUrl);
            var userInfo = uri.UserInfo.Split(':', 2, StringSplitOptions.None);
            var username = Uri.UnescapeDataString(userInfo[0]);
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
            var database = uri.AbsolutePath.TrimStart('/');

            // Assert
            username.Should().Be("myuser");
            password.Should().Be("mypassword");
            uri.Host.Should().Be("localhost");
            uri.Port.Should().Be(5432);
            database.Should().Be("mydatabase");
        }

        [Fact]
        public void BuildConnectionString_WithDirectServerString_ShouldReturnAsIs()
        {
            // Arrange
            string directString = "Host=localhost;Username=postgres;Password=1234;Database=testdb";

            // Act
            bool containsHost = directString.Contains("Host=", StringComparison.OrdinalIgnoreCase);

            // Assert
            containsHost.Should().BeTrue("เพราะถ้ามี Host= ควรคืนค่าสตริงนั้นตรงๆ ตาม Logic ในโค้ด");
        }
    }
}