using AutoMapper;
using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;
using HotPot23API.Services;
using Moq;
using NUnit.Framework;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HotPot23APITest
{
    public class AuthenticationServiceTest
    {
        private Mock<IRepository<int, UserMaster>> _userMasterRepoMock;
        private Mock<IRepository<string, User>> _userRepoMock;
        private Mock<IRepository<int, UserAddressMaster>> _addressRepoMock;
        private Mock<IRepository<int, RestaurantMaster>> _restaurantRepoMock;
        private Mock<ITokenService> _tokenServiceMock;
        private IMapper _mapper;
        private AuthenticationService _authService;

        [SetUp]
        public void Setup()
        {
            _userMasterRepoMock = new Mock<IRepository<int, UserMaster>>();
            _userRepoMock = new Mock<IRepository<string, User>>();
            _addressRepoMock = new Mock<IRepository<int, UserAddressMaster>>();
            _restaurantRepoMock = new Mock<IRepository<int, RestaurantMaster>>();
            _tokenServiceMock = new Mock<ITokenService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserRegisterDTO, UserMaster>();
            });

            _mapper = config.CreateMapper();

            _authService = new AuthenticationService(
                _userMasterRepoMock.Object,
                _userRepoMock.Object,
                _addressRepoMock.Object,
                _restaurantRepoMock.Object,
                _tokenServiceMock.Object,
                _mapper
            );
        }

        [Test]
        public async Task Register_ShouldReturnResponse_WhenDataIsValid()
        {
            var registerDTO = new UserRegisterDTO
            {
                Username = "testuser",
                Role = "User",
                AddressLine = "123 Main St",
                City = "Chennai",
                State = "TN",
                Pincode = "600001",
                Landmark = "Near Park",
                AddressType = "Home",
                IsDefault = true
            };

            var createdUserMaster = new UserMaster
            {
                UserID = 1,
                Username = registerDTO.Username,
                Role = registerDTO.Role,
                IsActive = true
            };

            _userMasterRepoMock.Setup(r => r.Add(It.IsAny<UserMaster>())).ReturnsAsync(createdUserMaster);
            _addressRepoMock.Setup(r => r.Add(It.IsAny<UserAddressMaster>())).ReturnsAsync(new UserAddressMaster());
            _userRepoMock.Setup(r => r.Add(It.IsAny<User>())).ReturnsAsync(new User());

            var result = await _authService.Register(registerDTO);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.UserID, Is.EqualTo(1));
            Assert.That(result.Username, Is.EqualTo("testuser"));
            Assert.That(result.Message, Is.EqualTo("Registered Successfully"));
        }

        [Test]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreCorrect()
        {
            string username = "testuser";
            string password = "testpass";
            var hmac = new HMACSHA256();
            byte[] hashKey = hmac.Key;
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

            var dbUser = new User
            {
                Username = username,
                Role = "User",
                HashKey = hashKey,
                Password = passwordHash,
                UserID = 1
            };

            _userRepoMock.Setup(r => r.GetById(username)).ReturnsAsync(dbUser);

            _restaurantRepoMock.Setup(r => r.GetAll())
                               .ReturnsAsync(new List<RestaurantMaster>());

            _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<TokenUser>()))
                             .ReturnsAsync("sample.jwt.token");

            var loginRequest = new LoginRequestDTO
            {
                Username = username,
                Password = password
            };

            var result = await _authService.Login(loginRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Username, Is.EqualTo(username));
            Assert.That(result.Token, Is.EqualTo("sample.jwt.token"));
        }

        [Test]
        public void Login_ShouldThrowException_WhenPasswordIsIncorrect()
        {
            var hmac = new HMACSHA256();
            byte[] hashKey = hmac.Key;
            byte[] actualPasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("correctpass"));

            var dbUser = new User
            {
                Username = "wronguser",
                Role = "User",
                HashKey = hashKey,
                Password = actualPasswordHash
            };

            _userRepoMock.Setup(r => r.GetById("wronguser")).ReturnsAsync(dbUser);

            var loginRequest = new LoginRequestDTO
            {
                Username = "wronguser",
                Password = "wrongpass"
            };

            Assert.ThrowsAsync<NoSuchEntityException>(() => _authService.Login(loginRequest));
        }

        [Test]
        public void Login_ShouldThrowException_WhenUserNotFound()
        {
            _userRepoMock.Setup(r => r.GetById("nouser")).ReturnsAsync((User)null);

            var loginRequest = new LoginRequestDTO
            {
                Username = "nouser",
                Password = "somepass"
            };

            Assert.ThrowsAsync<NoSuchEntityException>(() => _authService.Login(loginRequest));
        }

        [Test]
        public void Register_ShouldThrowException_WhenUserMasterFails()
        {
            _userMasterRepoMock.Setup(r => r.Add(It.IsAny<UserMaster>()))
                .ThrowsAsync(new Exception("DB insert failed"));

            var dto = new UserRegisterDTO
            {
                Username = "failuser",
                Role = "User",
                AddressLine = "Line",
                City = "City",
                State = "State",
                Pincode = "123",
                Landmark = "Landmark",
                IsDefault = true
            };

            Assert.ThrowsAsync<Exception>(() => _authService.Register(dto));
        }
    }
}
