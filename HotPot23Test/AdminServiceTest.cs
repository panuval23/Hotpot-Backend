using AutoMapper;
using HotPot23API.Contexts;
using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;
using HotPot23API.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotPot23APITest
{
    public class AdminServiceTest
    {
        private FoodDeliveryManagementContext _context;
        private Mock<IMapper> _mapperMock;
        private IAdminService _adminService;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<FoodDeliveryManagementContext>()
                .UseInMemoryDatabase(databaseName: "AdminServiceTestDB")
                .Options;

            _context = new FoodDeliveryManagementContext(options);
            _mapperMock = new Mock<IMapper>();

            var user1 = new UserMaster { UserID = 1, Username = "user1", Role = "User", IsActive = true };
            var user2 = new UserMaster { UserID = 2, Username = "restoOwner", Role = "Restaurant", IsActive = true };
            _context.UserMasters.AddRange(user1, user2);

            var resto1 = new RestaurantMaster { RestaurantID = 1, RestaurantName = "TestResto", IsActive = true, UserID = 2 };
            _context.RestaurantMasters.Add(resto1);

            await _context.SaveChangesAsync();

            _adminService = new AdminService(_context, _mapperMock.Object);
        }

       

        [Test]
        public async Task AddRestaurantAsync_WithExistingUser_ShouldReturnResponseDTO()
        {
            
            var dto = new AdminRestaurantRegisterDTO { UserId = 2 };
            _mapperMock.Setup(m => m.Map<RestaurantMaster>(It.IsAny<AdminRestaurantRegisterDTO>()))
                .Returns(new RestaurantMaster { RestaurantName = "MappedResto" });
            _mapperMock.Setup(m => m.Map<RestaurantResponseDTO>(It.IsAny<RestaurantMaster>()))
                .Returns(new RestaurantResponseDTO());

           
            var result = await _adminService.AddRestaurantAsync(dto);

     
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo("Restaurant registered successfully"));
        }

        [Test]
        public async Task UpdateRestaurantAsync_ShouldUpdateAndReturnDTO()
        {
            
            var dto = new AdminRestaurantRegisterDTO { OwnerName = "UpdatedOwner" };
            _mapperMock.Setup(m => m.Map(It.IsAny<AdminRestaurantRegisterDTO>(), It.IsAny<RestaurantMaster>()));
            _mapperMock.Setup(m => m.Map<RestaurantResponseDTO>(It.IsAny<RestaurantMaster>()))
                .Returns(new RestaurantResponseDTO());

        
            var result = await _adminService.UpdateRestaurantAsync(1, dto);

       
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo("Restaurant updated successfully"));
        }

        [Test]
        public async Task DeleteRestaurantAsync_ShouldReturnTrue()
        {
            
            var result = await _adminService.DeleteRestaurantAsync(1);

         
            Assert.That(result, Is.True);
            Assert.That(_context.RestaurantMasters.First(r => r.RestaurantID == 1).IsActive, Is.False);
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnPaginatedUsers()
        {
            
            _mapperMock.Setup(m => m.Map<List<UserDTO>>(It.IsAny<List<UserMaster>>()))
                .Returns(new List<UserDTO> { new UserDTO { Username = "user1" } });

            var result = await _adminService.GetAllUsersAsync(1, 10);

            Assert.That(result.Users, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task GetAllRestaurantsAsync_ShouldReturnPaginatedRestaurants()
        {
          
            _mapperMock.Setup(m => m.Map<List<RestaurantResponseDTO>>(It.IsAny<List<RestaurantMaster>>()))
                .Returns(new List<RestaurantResponseDTO> { new RestaurantResponseDTO { RestaurantName = "TestResto" } });

       
            var result = await _adminService.GetAllRestaurantsAsync(1, 10);

           
            Assert.That(result.Restaurants, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task AddRestaurant_NewUser_Success()
        {
            var options = new DbContextOptionsBuilder<FoodDeliveryManagementContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new FoodDeliveryManagementContext(options);

            var mapper = new Mock<IMapper>();

        
            mapper.Setup(m => m.Map<RestaurantMaster>(It.IsAny<AdminRestaurantRegisterDTO>()))
                .Returns((AdminRestaurantRegisterDTO dto) => new RestaurantMaster
                {
                    RestaurantName = dto.RestaurantName,
                    CuisineType = dto.CuisineType,
                    IsAvailable = true
                });

            mapper.Setup(m => m.Map<RestaurantResponseDTO>(It.IsAny<RestaurantMaster>()))
                .Returns((RestaurantMaster r) => new RestaurantResponseDTO
                {
                    RestaurantID = r.RestaurantID,
                    RestaurantName = r.RestaurantName,
                    
                });

            var service = new AdminService(context, mapper.Object);

            var dto = new AdminRestaurantRegisterDTO
            {
                RestaurantName = "New Test Restaurant",
                CuisineType = "Italian",
                Username = "newrestuser",
                ContactNumber = "1234567890",
                OwnerName = "Test Owner",
                Email = "test@example.com",
                Gender = "Male"
            };

          
            var result = await service.AddRestaurantAsync(dto);

          
            Assert.IsNotNull(result);
            Assert.AreEqual("New Test Restaurant", result.RestaurantName);

            var createdUser = context.UserMasters.FirstOrDefault(u => u.Username == "newrestuser");
            Assert.IsNotNull(createdUser, "User should be created for new restaurant");
            Assert.AreEqual("Restaurant", createdUser.Role);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

    }
}
