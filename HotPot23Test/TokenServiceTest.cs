using HotPot23API.Interfaces;
using HotPot23API.Models.DTOs;
using HotPot23API.Services;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPot23APITest
{
    public class TokenServiceTest
    {
        private ITokenService _tokenService;

        [SetUp]
        public void Setup()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["Tokens:JWT"] = "ThisIsASecretKeyForUnitTestingTokenService"
                })
                .Build();

            _tokenService = new TokenService(config);
        }

        [Test]
        public async Task GenerateToken_NoOptional()
        {
            // Arrange
            var user = new TokenUser
            {
                Username = "TestUser",
                Role = "Tester"
            };

            // Act
            var token = await _tokenService.GenerateToken(user);

            // Assert
            Assert.That(token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task GenerateToken_WithOptional()
        {
            // Arrange
            var user = new TokenUser
            {
                Username = "RestaurantOwner",
                Role = "Restaurant",
                RestaurantID = 101,
                UserID = 55
            };

            // Act
            var token = await _tokenService.GenerateToken(user);

            // Assert
            Assert.That(token, Is.Not.Null.And.Not.Empty);
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up if needed
        }
    }
}
