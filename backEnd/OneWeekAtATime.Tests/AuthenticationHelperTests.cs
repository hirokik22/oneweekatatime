using Microsoft.VisualStudio.TestTools.UnitTesting;
using WeeklyPlanner.API.Middleware;

namespace OneWeekAtATime.Tests
{
    [TestClass]
    public class AuthenticationHelperTests
    {
        [TestMethod]
        public void Encrypt_ShouldReturnBase64EncodedHeader()
        {
            // Arrange
            string username = "testUser";
            string password = "password123";

            // Act
            string result = AuthenticationHelper.Encrypt(username, password);

            // Assert
            Assert.AreEqual("Basic dGVzdFVzZXI6cGFzc3dvcmQxMjM=", result);
        }

        [TestMethod]
        public void Decrypt_ShouldReturnOriginalUsernameAndPassword()
        {
            // Arrange
            string encodedHeader = "Basic dGVzdFVzZXI6cGFzc3dvcmQxMjM=";

            // Act
            AuthenticationHelper.Decrypt(encodedHeader, out string username, out string password);

            // Assert
            Assert.AreEqual("testUser", username);
            Assert.AreEqual("password123", password);
        }
    }
}
