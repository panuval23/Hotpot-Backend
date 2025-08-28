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
    public class RestaurantServiceTest
    {
        private FoodDeliveryManagementContext _context;
        private Mock<IMapper> _mapperMock;
        private IRestaurantService _restaurantService;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<FoodDeliveryManagementContext>()
                .UseInMemoryDatabase(databaseName: "HotPotRestaurantTestDB") 
                .Options;

            _context = new FoodDeliveryManagementContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

           
            var restaurant = new RestaurantMaster { RestaurantID = 100, RestaurantName = "Testaurant" };
            var category = new CategoryMaster { CategoryID = 100, CategoryName = "Starters", RestaurantID = 100 };
            var menuItem = new MenuItemMaster { MenuItemID = 100, Name = "Paneer Tikka", RestaurantID = 100, CategoryID = 100, Price = 200 };
            var order = new OrderTransaction
            {
                OrderID = 100,
                RestaurantID = 100,
                OrderStatus = "Pending",
                TotalAmount = 100,
                CreatedOn = DateTime.UtcNow,
                OrderItems = new List<OrderItemDetails>
                {
                    new OrderItemDetails { MenuItemID = 100, Quantity = 1, PriceAtOrder = 100, MenuItem = menuItem }
                },
                Restaurant = restaurant
            };
            await _context.RestaurantMasters.AddAsync(restaurant);
            await _context.CategoryMasters.AddAsync(category);
            await _context.MenuItems.AddAsync(menuItem);
            await _context.OrderTransactions.AddAsync(order);
            await _context.SaveChangesAsync();

            #region Mapper mock setup
            _mapperMock = new Mock<IMapper>();
            _mapperMock.Setup(m => m.Map<CategoryResponseDTO>(It.IsAny<CategoryMaster>()))
                .Returns((CategoryMaster c) => c == null ? null : new CategoryResponseDTO { CategoryID = c.CategoryID, CategoryName = c.CategoryName });

            _mapperMock.Setup(m => m.Map<List<CategoryResponseDTO>>(It.IsAny<List<CategoryMaster>>()))
                .Returns((List<CategoryMaster> src) => src?.Select(c => new CategoryResponseDTO { CategoryID = c.CategoryID, CategoryName = c.CategoryName }).ToList());

            _mapperMock.Setup(m => m.Map<MenuItemResponseDTO>(It.IsAny<MenuItemMaster>()))
                .Returns((MenuItemMaster m) => m == null ? null : new MenuItemResponseDTO { MenuItemID = m.MenuItemID, Name = m.Name, Price = m.Price });

            _mapperMock.Setup(m => m.Map<List<MenuItemResponseDTO>>(It.IsAny<List<MenuItemMaster>>()))
                .Returns((List<MenuItemMaster> src) => src?.Select(mi => new MenuItemResponseDTO { MenuItemID = mi.MenuItemID, Name = mi.Name, Price = mi.Price }).ToList());

            _mapperMock.Setup(m => m.Map<MenuItemMaster>(It.IsAny<MenuItemCreateDTO>()))
                .Returns((MenuItemCreateDTO dto) => new MenuItemMaster { Name = dto.Name, Price = dto.Price, CategoryID = dto.CategoryID });

            _mapperMock.Setup(m => m.Map(It.IsAny<MenuItemUpdateDTO>(), It.IsAny<MenuItemMaster>()))
                .Callback((MenuItemUpdateDTO src, MenuItemMaster dest) =>
                {
                    dest.Name = src.Name;
                    dest.Price = src.Price;
                    dest.CategoryID = src.CategoryID;
                });

            _restaurantService = new RestaurantService(_context, _mapperMock.Object);
            #endregion
        }

        #region AddCategoryAsync
        [Test]
        public async Task AddCategoryAsync_Success()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            await _context.RestaurantMasters.AddAsync(restaurant);
            await _context.SaveChangesAsync();

            var dto = new CategoryCreateDTO { CategoryName = "Main Course", IsActive = true };

            var result = await _restaurantService.AddCategoryAsync(1, dto);

          
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CategoryName, Is.EqualTo("Main Course"));
            Assert.That(_context.CategoryMasters.Count(c => c.RestaurantID == 1), Is.EqualTo(1));
        }
        [Test]
        public void AddCategoryAsync_Exception()
        {
            
            var dto = new CategoryCreateDTO { CategoryName = "Main Course", IsActive = true };

       
            var ex = Assert.ThrowsAsync<NoSuchEntityException>(
                () => _restaurantService.AddCategoryAsync(999, dto)
            );
            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }




        #endregion



        #region UpdateCategoryAsync
        [Test]
        public async Task UpdateCategoryAsync_Success()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var category = new CategoryMaster { CategoryID = 1, RestaurantID = 1, CategoryName = "Old Name", IsActive = true };

            _context.RestaurantMasters.Add(restaurant);
            _context.CategoryMasters.Add(category);
            await _context.SaveChangesAsync();

            var updateDto = new CategoryUpdateDTO { CategoryName = "New Name", IsActive = false };

            
            var result = await _restaurantService.UpdateCategoryAsync(1, 1, updateDto);

      
            Assert.That(result.CategoryName, Is.EqualTo("New Name"));
            Assert.That(result.IsActive, Is.False);
        }

        [Test]
        public void UpdateCategoryAsync_NotFound_Throws()
        {
         
            var updateDto = new CategoryUpdateDTO { CategoryName = "New Name", IsActive = false };

            var ex = Assert.ThrowsAsync<NoSuchEntityException>(
                () => _restaurantService.UpdateCategoryAsync(1, 999, updateDto)
            );

            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }
        #endregion


        #region DeleteCategoryAsync
        [Test]

        public async Task DeleteCategoryAsync_Success()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var category = new CategoryMaster { CategoryID = 1, RestaurantID = 1, CategoryName = "Main Course" };

            await _context.RestaurantMasters.AddAsync(restaurant);
            await _context.CategoryMasters.AddAsync(category);
            await _context.SaveChangesAsync();

      
            var result = await _restaurantService.DeleteCategoryAsync(1, 1);

          
            Assert.That(result, Is.True);
            Assert.That(_context.CategoryMasters.Count(c => c.RestaurantID == 1), Is.EqualTo(0));
        }


        [Test]
        public void DeleteCategoryAsync_Exception()
        {
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            _context.RestaurantMasters.Add(restaurant);
            _context.SaveChanges();

           
            var ex = Assert.ThrowsAsync<NoSuchEntityException>(
                () => _restaurantService.DeleteCategoryAsync(1, 999)
            );

            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }
        #endregion

        #region GetCategoriesByRestaurantAsync
        [Test]
        public async Task GetCategoriesByRestaurantAsync_Success()
        {
          
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var category = new CategoryMaster { CategoryID = 1, RestaurantID = 1, CategoryName = "Brunch", IsActive = true };

            _context.RestaurantMasters.Add(restaurant);
            _context.CategoryMasters.Add(category);
            await _context.SaveChangesAsync();

         
            var result = await _restaurantService.GetCategoriesByRestaurantAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().CategoryName, Is.EqualTo("Brunch"));
        }

        [Test]
        public void GetCategoriesByRestaurantAsync_NoCategories_Throws()
        {
           
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            _context.RestaurantMasters.Add(restaurant);
            _context.SaveChanges();

        
            var ex = Assert.ThrowsAsync<NoEntriessInCollectionException>(
                () => _restaurantService.GetCategoriesByRestaurantAsync(1)
            );

            Assert.That(ex.Message, Is.EqualTo("Collection is empty"));
        }
        #endregion

        #region AddMenuItemAsync
        [Test]
        public async Task AddMenuItemAsync_Success()
        {
           
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var category = new CategoryMaster { CategoryID = 1, CategoryName = "Starters", RestaurantID = 1 };

            _context.RestaurantMasters.Add(restaurant);
            _context.CategoryMasters.Add(category);
            await _context.SaveChangesAsync();

            var dto = new MenuItemCreateDTO
            {
                CategoryID = 1,
                Name = "Pizza",
                Price = 10.5M
            };

         
            var result = await _restaurantService.AddMenuItemAsync(1, dto);

       
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Pizza"));
            Assert.That(result.Price, Is.EqualTo(10.5M));
        }

        [Test]
        public void AddMenuItemAsync_RestaurantNotFound_Throws()
        {
          
            var dto = new MenuItemCreateDTO
            {
                CategoryID = 1,
                Name = "Pizza",
                Price = 10.5M
            };

          
            var ex = Assert.ThrowsAsync<NoSuchEntityException>(() => _restaurantService.AddMenuItemAsync(999, dto));
            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }

        [Test]
        public async Task AddMenuItemAsync_CategoryNotBelongsToRestaurant_Throws()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var category = new CategoryMaster { CategoryID = 2, CategoryName = "Starters", RestaurantID = 2 }; 

            _context.RestaurantMasters.Add(restaurant);
            _context.CategoryMasters.Add(category);
            await _context.SaveChangesAsync();

            var dto = new MenuItemCreateDTO
            {
                CategoryID = 2,
                Name = "Burger",
                Price = 5.0M
            };

            var ex = Assert.ThrowsAsync<NoSuchEntityException>(() => _restaurantService.AddMenuItemAsync(1, dto));
            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }
        #endregion

        #region  UpdateMenuItemAsync
        [Test]
        public async Task UpdateMenuItemAsync_Success()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var category = new CategoryMaster { CategoryID = 1, CategoryName = "Brunch", RestaurantID = 1 };
            var menuItem = new MenuItemMaster { MenuItemID = 1, RestaurantID = 1, Name = "Pasta", Price = 10.0M, CategoryID = 1 };

            _context.RestaurantMasters.Add(restaurant);
            _context.CategoryMasters.Add(category);
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var updateDto = new MenuItemUpdateDTO { Name = "Updated Pasta", Price = 12.5M, CategoryID = 1 };

         
            var result = await _restaurantService.UpdateMenuItemAsync(1, 1, updateDto);

         
            Assert.That(result.Name, Is.EqualTo("Updated Pasta"));
            Assert.That(result.Price, Is.EqualTo(12.5M));
        }

        [Test]
        public void UpdateMenuItemAsync_ItemNotFound_Throws()
        {
       
            var updateDto = new MenuItemUpdateDTO { Name = "Updated Pasta", Price = 12.5M, CategoryID = 1 };

           
            var ex = Assert.ThrowsAsync<NoSuchEntityException>(
                () => _restaurantService.UpdateMenuItemAsync(1, 999, updateDto)
            );

            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }

        [Test]
        public async Task UpdateMenuItemAsync_CategoryNotFound_Throws()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var menuItem = new MenuItemMaster { MenuItemID = 1, RestaurantID = 1, Name = "Pasta", Price = 10.0M, CategoryID = 1 };

            _context.RestaurantMasters.Add(restaurant);
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var updateDto = new MenuItemUpdateDTO { Name = "Updated Pasta", Price = 12.5M, CategoryID = 99 }; // Non-existent category

        
            var ex = Assert.ThrowsAsync<NoSuchEntityException>(
                () => _restaurantService.UpdateMenuItemAsync(1, 1, updateDto)
            );

            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }
        #endregion

        #region DeleteMenuItemAsync
        [Test]
        public async Task DeleteMenuItemAsync_WhenMenuItemExists_ShouldRemoveMenuItem()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var menuItem = new MenuItemMaster { MenuItemID = 1, RestaurantID = 1, Name = "Sandwich", Price = 5.0M };

            await _context.RestaurantMasters.AddAsync(restaurant);
            await _context.MenuItems.AddAsync(menuItem);
            await _context.SaveChangesAsync();

          
            var result = await _restaurantService.DeleteMenuItemAsync(1, 1);

         
            Assert.That(result, Is.True);
            Assert.That(_context.MenuItems.Count(m => m.RestaurantID == 1), Is.EqualTo(0));
        }

        [Test]
        public void DeleteMenuItemAsync_NotFound_Throws()
        {
            var ex = Assert.ThrowsAsync<NoSuchEntityException>(
                () => _restaurantService.DeleteMenuItemAsync(1, 999)
            );

            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }
        #endregion


        #region GetAllMenuItemsAsync
        [Test]
        public async Task GetAllMenuItemsAsync_ReturnsItems()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var menuItem = new MenuItemMaster { MenuItemID = 1, RestaurantID = 1, Name = "Pizza", Price = 15.0M };

            _context.RestaurantMasters.Add(restaurant);
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            var result = await _restaurantService.GetAllMenuItemsAsync(1);

      
            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().Name, Is.EqualTo("Pizza"));
        }

        [Test]
        public void GetAllMenuItemsAsync_NoItems_Throws()
        {
            
            var ex = Assert.ThrowsAsync<NoEntriessInCollectionException>(
                () => _restaurantService.GetAllMenuItemsAsync(1)
            );

            Assert.That(ex.Message, Is.EqualTo("Collection is empty"));
        }
        #endregion

        #region GetMenuItemByIdAsync
        [Test]
        public async Task GetMenuItemByIdAsync_ReturnsItem()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var menuItem = new MenuItemMaster { MenuItemID = 1, RestaurantID = 1, Name = "Pasta", Price = 12.5M };

            _context.RestaurantMasters.Add(restaurant);
            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

           
            var result = await _restaurantService.GetMenuItemByIdAsync(1, 1);

          
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Pasta"));
            Assert.That(result.Price, Is.EqualTo(12.5M));
        }

        [Test]
        public void GetMenuItemByIdAsync_NotFound_Throws()
        {
            
            var ex = Assert.ThrowsAsync<NoSuchEntityException>(
                () => _restaurantService.GetMenuItemByIdAsync(1, 999)
            );

            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }
        #endregion


        // 10 AddDiscountAsync
        [Test]
        public void AddDiscount_Success()
        {
            Assert.DoesNotThrowAsync(() => _restaurantService.AddDiscountAsync(1, new AddDiscountDTO { MenuItemID = 1, DiscountPercent = 10 }));
        }

        [Test]
        public async Task AddDiscount_NoValidationStillPasses()
        {
            await _restaurantService.AddDiscountAsync(1, new AddDiscountDTO { MenuItemID = 999, DiscountPercent = 10 });
            Assert.Pass();
        }

        #region GetCurrentOrdersAsync
        [Test]
        public async Task GetCurrentOrdersAsync_ReturnsOrders()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var order = new OrderTransaction
            {
                OrderID = 1,
                RestaurantID = 1,
                OrderStatus = "Pending",
                CreatedOn = DateTime.UtcNow,
                TotalAmount = 100,
                Restaurant = restaurant,
                OrderItems = new List<OrderItemDetails>
        {
            new OrderItemDetails
            {
                MenuItemID = 1,
                PriceAtOrder = 100,
                Quantity = 1,
                MenuItem = new MenuItemMaster { MenuItemID = 1, Name = "Pizza", Price = 100 }
            }
        }
            };

            _context.RestaurantMasters.Add(restaurant);
            _context.OrderTransactions.Add(order);
            await _context.SaveChangesAsync();

            
            var result = await _restaurantService.GetCurrentOrdersAsync(1);

        
            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().Status, Is.EqualTo("Pending"));
        }

        [Test]
        public void GetCurrentOrdersAsync_NoOrders_Throws()
        {
            
            var ex = Assert.ThrowsAsync<NoEntriessInCollectionException>(
                () => _restaurantService.GetCurrentOrdersAsync(1)
            );

            Assert.That(ex.Message, Is.EqualTo("Collection is empty"));
        }
        #endregion


        #region GetOrderHistoryAsync
        [Test]
        
        public async Task GetOrderHistoryAsync_ReturnsOrders()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var order = new OrderTransaction
            {
                OrderID = 1,
                RestaurantID = 1,
                OrderStatus = "Delivered",
                CreatedOn = DateTime.UtcNow,
                TotalAmount = 50,
                Restaurant = restaurant,
                OrderItems = new List<OrderItemDetails>
        {
            new OrderItemDetails
            {
                MenuItemID = 1,
                PriceAtOrder = 50,
                Quantity = 1,
                MenuItem = new MenuItemMaster { MenuItemID = 1, Name = "Burger", Price = 50 }
            }
        },
                DeliveryStatuses = new List<DeliveryStatusTransaction>
        {
            new DeliveryStatusTransaction
            {
                Status = "Delivered",
                UpdatedAt = DateTime.UtcNow,
                StatusUpdatedBy = 101
            }
        }
            };

            _context.RestaurantMasters.Add(restaurant);
            _context.OrderTransactions.Add(order);
            await _context.SaveChangesAsync();

           
            var result = await _restaurantService.GetOrderHistoryAsync(1);

           
            Assert.That(result.Items.Count, Is.EqualTo(1));
            Assert.That(result.Items.First().Status, Is.EqualTo("Delivered"));
        }

        [Test]
        public void GetOrderHistoryAsync_NoOrders_Throws()
        {
          
            var ex = Assert.ThrowsAsync<NoEntriessInCollectionException>(
                () => _restaurantService.GetOrderHistoryAsync(1)
            );

            Assert.That(ex.Message, Is.EqualTo("Collection is empty"));
        }
        #endregion

        #region UpdateOrderStatusAsync
        [Test]
        public async Task UpdateOrderStatusAsync_Success()
        {
            
            var restaurant = new RestaurantMaster { RestaurantID = 1, RestaurantName = "Test Restaurant" };
            var order = new OrderTransaction
            {
                OrderID = 1,
                RestaurantID = 1,
                OrderStatus = "Pending",
                CreatedOn = DateTime.UtcNow,
                TotalAmount = 50,
                OrderItems = new List<OrderItemDetails>
        {
            new OrderItemDetails
            {
                MenuItemID = 1,
                PriceAtOrder = 50,
                Quantity = 1,
                MenuItem = new MenuItemMaster { MenuItemID = 1, Name = "Burger", Price = 50 }
            }
        },
                Restaurant = restaurant
            };

            _context.RestaurantMasters.Add(restaurant);
            _context.OrderTransactions.Add(order);
            await _context.SaveChangesAsync();

         
            var result = await _restaurantService.UpdateOrderStatusAsync(1, 1, "Completed", 101);

         
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Status, Is.EqualTo("Completed"));
            Assert.That(result.DeliveryStatuses.Count, Is.EqualTo(1));
            Assert.That(result.DeliveryStatuses.First().Status, Is.EqualTo("Completed"));
        }

        [Test]
        public void UpdateOrderStatusAsync_OrderNotFound_Throws()
        {
            
            var ex = Assert.ThrowsAsync<NoSuchEntityException>(
                () => _restaurantService.UpdateOrderStatusAsync(1, 999, "Completed", 101)
            );

            Assert.That(ex.Message, Is.EqualTo("Entity with the given Id not present"));
        }
        #endregion


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
