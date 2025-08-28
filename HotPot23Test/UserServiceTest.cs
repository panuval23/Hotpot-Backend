using AutoMapper;
using HotPot23API.Contexts;
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
    public class UserServiceTest
    {
        private FoodDeliveryManagementContext _context;
        private Mock<IMapper> _mapperMock;
        private IUserService _userService;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<FoodDeliveryManagementContext>()
                .UseInMemoryDatabase(databaseName: "HotPotTestDB")
                .Options;

            _context = new FoodDeliveryManagementContext(options);

         
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            #region Seed basic data
            var restaurant1 = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Testaurant 1" };
            var category1 = new CategoryMaster { CategoryID = 1, CategoryName = "Starters", RestaurantID = 1 };
            var menuItem1 = new MenuItemMaster { MenuItemID = 1, Name = "Paneer Tikka", Price = 200, IsVeg = true, RestaurantID = 1, CategoryID = 1 };
            var menuItem2 = new MenuItemMaster { MenuItemID = 2, Name = "Chicken Curry", Price = 300, IsVeg = false, RestaurantID = 1, CategoryID = 1 };
            
            await _context.RestaurantMasters.AddAsync(restaurant1);
            await _context.CategoryMasters.AddAsync(category1);
            await _context.MenuItems.AddRangeAsync(menuItem1, menuItem2);
            await _context.SaveChangesAsync();
            #endregion

            #region Mock AutoMapper
            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(m => m.Map<IEnumerable<UserRestaurantDTO>>(It.IsAny<IEnumerable<RestaurantMaster>>()))
                .Returns((IEnumerable<RestaurantMaster> src) => src.Select(r => new UserRestaurantDTO { RestaurantID = r.RestaurantID, Name = r.RestaurantName }));

            _mapperMock.Setup(m => m.Map<IEnumerable<UserMenuItemResponseDTO>>(It.IsAny<IEnumerable<MenuItemMaster>>()))
                .Returns((IEnumerable<MenuItemMaster> src) => src.Select(mi => new UserMenuItemResponseDTO { MenuItemID = mi.MenuItemID, Name = mi.Name, Price = mi.Price }));

            _mapperMock.Setup(m => m.Map<UserMenuItemResponseDTO>(It.IsAny<MenuItemMaster>()))
                .Returns((MenuItemMaster mi) => mi == null ? null : new UserMenuItemResponseDTO
                {
                    MenuItemID = mi.MenuItemID,
                    Name = mi.Name,
                    Price = mi.Price
                });

            _userService = new UserService(_context, _mapperMock.Object);
            #endregion
        }

        #region GetAllRestaurantsAsync
        //[Test]
        //public async Task GetAllRestaurants_Success()
        //{
        //    var result = await _userService.GetAllRestaurantsAsync();
        //    Assert.That(result.Count(), Is.EqualTo(1));
        //}

        //[Test]
        //public async Task GetAllRestaurants_Exception()
        //{
        //    _context.Dispose();
        //    Assert.ThrowsAsync<ObjectDisposedException>(() => _userService.GetAllRestaurantsAsync());
        //}
        #endregion

        #region GetMenuByRestaurantAsync
        //[Test]
        //public async Task GetMenuByRestaurant_Success()
        //{
        //    var result = await _userService.GetMenuByRestaurantAsync("Testaurant 1");
        //    Assert.That(result.Count(), Is.EqualTo(2));
        //}

        //[Test]
        //public async Task GetMenuByRestaurant_Exception()
        //{
        //    var result = await _userService.GetMenuByRestaurantAsync("NonExisting");
        //    Assert.That(result, Is.Empty);
        //}
        #endregion

        #region SearchMenuItemsAsync
        //[Test]
        //public async Task SearchMenuItems_Success()
        //{
        //    var result = await _userService.SearchMenuItemsAsync("Paneer");
        //    Assert.That(result.Count(), Is.EqualTo(1));
        //}

        //[Test]
        //public void SearchMenuItems_Exception()
        //{
        //    var ex = Assert.ThrowsAsync<ArgumentNullException>(() => _userService.SearchMenuItemsAsync(null));
        //    Assert.That(ex.ParamName, Is.EqualTo("searchTerm"));
        //    Assert.That(ex.Message, Does.Contain("cannot be null or empty"));
        //}
        #endregion

        #region GetMenuItemDetailsAsync
        [Test]
        public async Task GetMenuItemDetails_Success()
        {
            var result = await _userService.GetMenuItemDetailsAsync(1);
            Assert.That(result.Name, Is.EqualTo("Paneer Tikka"));
        }

        [Test]
        public async Task GetMenuItemDetails_Exception()
        {
            var result = await _userService.GetMenuItemDetailsAsync(999);
            Assert.That(result, Is.Null);
        }
        #endregion

        #region AddToCartAsync
        [Test]
        public async Task AddToCart_Success()
        {
            var dto = new AddToCartDTO { MenuItemID = 1, RestaurantID = 1, Quantity = 2 };
            var result = await _userService.AddToCartAsync(1, dto);
            Assert.That(result.Quantity, Is.EqualTo(2));
        }

        [Test]
        public void AddToCart_Exception()
        {
            var dto = new AddToCartDTO { MenuItemID = 1, RestaurantID = 1, Quantity = 0 };
            Assert.ThrowsAsync<ArgumentException>(() => _userService.AddToCartAsync(1, dto));
        }
        #endregion

        #region GetCartAsync
        [Test]
        public async Task GetCart_Success()
        {
            await _userService.AddToCartAsync(1, new AddToCartDTO { MenuItemID = 1, RestaurantID = 1, Quantity = 1 });
            var result = await _userService.GetCartAsync(1);
            Assert.That(result.Items.Count, Is.EqualTo(1));
        }

        [Test]
        public async Task GetCart_Exception()
        {
            var result = await _userService.GetCartAsync(99);
            Assert.That(result.Items, Is.Empty);
        }
        #endregion

        #region UpdateCartItemAsync
        [Test]
        public async Task UpdateCartItem_Success()
        {
            var added = await _userService.AddToCartAsync(1, new AddToCartDTO { MenuItemID = 1, RestaurantID = 1, Quantity = 1 });
            var result = await _userService.UpdateCartItemAsync(1, new UpdateCartItemDTO { CartID = added.CartID, Quantity = 5 });
            Assert.That(result.Quantity, Is.EqualTo(5));
        }

        [Test]
        public void UpdateCartItem_Exception()
        {
            Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.UpdateCartItemAsync(1, new UpdateCartItemDTO { CartID = 999, Quantity = 1 }));
        }
        #endregion

        #region RemoveCartItemAsync
        [Test]
        public async Task RemoveCartItem_Success()
        {
            var added = await _userService.AddToCartAsync(1, new AddToCartDTO { MenuItemID = 1, RestaurantID = 1, Quantity = 1 });
            var result = await _userService.RemoveCartItemAsync(1, added.CartID);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RemoveCartItem_Exception()
        {
            var result = await _userService.RemoveCartItemAsync(1, 999);
            Assert.That(result, Is.False);
        }
        #endregion

        #region CheckoutAsync
        [Test]
        public async Task Checkout_Success()
        {
            await _userService.AddToCartAsync(1, new AddToCartDTO { MenuItemID = 1, RestaurantID = 1, Quantity = 1 });
            var result = await _userService.CheckoutAsync(1, new CheckoutDTO { ShippingAddressID = 1, PaymentMethod = "COD" });
            Assert.That(result.TotalAmount, Is.GreaterThan(0));
        }

        [Test]
        public void Checkout_Exception()
        {
            Assert.ThrowsAsync<Exception>(() => _userService.CheckoutAsync(1, new CheckoutDTO { ShippingAddressID = 1, PaymentMethod = "COD" }));
        }
        #endregion

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
