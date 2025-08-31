using AutoMapper;
using HotPot23API.Contexts;
using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace HotPot23API.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly FoodDeliveryManagementContext _context;
        private readonly IMapper _mapper;

        public RestaurantService(FoodDeliveryManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Add new category for specific restaurant
        public async Task<CategoryResponseDTO?> AddCategoryAsync(int restaurantId, CategoryCreateDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "Category data cannot be null.");

            if (string.IsNullOrWhiteSpace(dto.CategoryName))
                throw new ArgumentException("Category name is required.", nameof(dto.CategoryName));

            var restaurantExists = await _context.RestaurantMasters
                .AnyAsync(r => r.RestaurantID == restaurantId);
            if (!restaurantExists)
                throw new NoSuchEntityException();

            var existing = await _context.CategoryMasters
                .FirstOrDefaultAsync(c => c.CategoryName == dto.CategoryName && c.RestaurantID == restaurantId);

            if (existing != null)
                return _mapper.Map<CategoryResponseDTO>(existing);

      
            var category = new CategoryMaster
            {
                CategoryName = dto.CategoryName,
                RestaurantID = restaurantId,
                IsActive = dto.IsActive,
                CreatedOn = DateTime.UtcNow
            };

            await _context.CategoryMasters.AddAsync(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDTO>(category);
        }


        #endregion


        #region Update category for a restaurant
        public async Task<CategoryResponseDTO?> UpdateCategoryAsync(int restaurantId, int categoryId, CategoryUpdateDTO dto)
        {
            var category = await _context.CategoryMasters
                .FirstOrDefaultAsync(c => c.CategoryID == categoryId && c.RestaurantID == restaurantId);

            if (category == null)
                throw new NoSuchEntityException(); 

            category.CategoryName = dto.CategoryName;
            category.IsActive = dto.IsActive;

            _context.CategoryMasters.Update(category);
            await _context.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDTO>(category);
        }
        #endregion

        #region Delete category of a restaurant
        public async Task<bool> DeleteCategoryAsync(int restaurantId, int categoryId)
        {
            var category = await _context.CategoryMasters
                .FirstOrDefaultAsync(c => c.CategoryID == categoryId && c.RestaurantID == restaurantId);

            if (category == null)
                throw new NoSuchEntityException(); 

            _context.CategoryMasters.Remove(category);
            await _context.SaveChangesAsync();
            return true;
        }
        #endregion

        #region GetCategoriesByRestaurantAsync
        public async Task<PaginatedResponseDTO<CategoryResponseDTO>> GetCategoriesByRestaurantAsync(
    int restaurantId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.CategoryMasters.Where(c => c.RestaurantID == restaurantId);

            var totalCount = await query.CountAsync();

            // ✅ Throw custom exception if no entries
            if (totalCount == 0)
                throw new NoEntriessInCollectionException();

            var categories = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedCategories = _mapper.Map<List<CategoryResponseDTO>>(categories);

            return new PaginatedResponseDTO<CategoryResponseDTO>
            {
                Items = mappedCategories,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        #endregion 



        #region AddMenuItemAsync
        public async Task<MenuItemResponseDTO?> AddMenuItemAsync(int restaurantId, MenuItemCreateDTO dto)
        {
            // ✅ Check restaurant existence
            var restaurantExists = await _context.RestaurantMasters
                .AnyAsync(r => r.RestaurantID == restaurantId);
            if (!restaurantExists)
                throw new NoSuchEntityException();

            // ✅ Check if category belongs to restaurant
            var categoryExists = await _context.CategoryMasters
                .AnyAsync(c => c.CategoryID == dto.CategoryID && c.RestaurantID == restaurantId);
            if (!categoryExists)
                throw new NoSuchEntityException();

            var menuItem = _mapper.Map<MenuItemMaster>(dto);
            menuItem.RestaurantID = restaurantId;
            menuItem.CreatedOn = DateTime.UtcNow;

            await _context.MenuItems.AddAsync(menuItem);
            await _context.SaveChangesAsync();

            return _mapper.Map<MenuItemResponseDTO>(menuItem);
        }
        #endregion


        #region Update a menu item belonging to the restaurant
        public async Task<MenuItemResponseDTO?> UpdateMenuItemAsync(int restaurantId, int menuItemId, MenuItemUpdateDTO dto)
        {
            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.MenuItemID == menuItemId && m.RestaurantID == restaurantId);
            if (menuItem == null) throw new NoSuchEntityException(); ;

            // Verify category belongs to restaurant if changed
            var categoryExists = await _context.CategoryMasters.AnyAsync(c => c.CategoryID == dto.CategoryID && c.RestaurantID == restaurantId);
            if (!categoryExists) throw new NoSuchEntityException(); ;

            _mapper.Map(dto, menuItem);

            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();

            return _mapper.Map<MenuItemResponseDTO>(menuItem);
        }
        #endregion

        #region Delete a menu item of the restaurant
        public async Task<bool> DeleteMenuItemAsync(int restaurantId, int menuItemId)
        {
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.MenuItemID == menuItemId && m.RestaurantID == restaurantId);

            if (menuItem == null)
                throw new NoSuchEntityException();

            menuItem.IsActive = false;
            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();

            return true; 
        }

        #endregion

        #region Get all menu items for a restaurant
        public async Task<PaginatedResponseDTO<MenuItemResponseDTO>> GetAllMenuItemsAsync(
    int restaurantId, int pageNumber = 1, int pageSize = 10)
        {
            // ✅ Only fetch active items
            var query = _context.MenuItems
                .Where(m => m.RestaurantID == restaurantId && m.IsActive);

            var totalCount = await query.CountAsync();
            if (totalCount == 0)
                throw new NoEntriessInCollectionException();

            var menuItems = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mappedMenuItems = _mapper.Map<List<MenuItemResponseDTO>>(menuItems);

            return new PaginatedResponseDTO<MenuItemResponseDTO>
            {
                Items = mappedMenuItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        #endregion

        #region Get single menu item by id for a restaurant
        public async Task<MenuItemResponseDTO?> GetMenuItemByIdAsync(int restaurantId, int menuItemId)
        {
            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.MenuItemID == menuItemId && m.RestaurantID == restaurantId);
            if (menuItem == null)
                throw new NoSuchEntityException();

            return _mapper.Map<MenuItemResponseDTO>(menuItem);
        }
        #endregion
        public async Task AddDiscountAsync(int restaurantId, AddDiscountDTO discountDto)
        {
            

            var discount = new DiscountMaster
            {
                MenuItemID = discountDto.MenuItemID,
                DiscountPercent = discountDto.DiscountPercent,
                ValidFrom = discountDto.ValidFrom,
                ValidTo = discountDto.ValidTo
            };

            await _context.DiscountMasters.AddAsync(discount);
            await _context.SaveChangesAsync();
        }
        #region GetCurrentOrdersAsync
        public async Task<PaginatedResponseDTO<OrderResponseDTO>> GetCurrentOrdersAsync(int restaurantId, int pageNumber = 1, int pageSize = 10)
        {
            var currentStatuses = new[] { "Pending", "Processing", "InProgress" };

            var query = _context.OrderTransactions
                .Where(o => o.RestaurantID == restaurantId && currentStatuses.Contains(o.OrderStatus))
                .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Restaurant);

            var totalCount = await query.CountAsync();
            if (totalCount == 0)
                throw new NoEntriessInCollectionException();

            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = orders.Select(o => new OrderResponseDTO
            {
                OrderID = o.OrderID,
                CreatedOn = o.CreatedOn,
                Status = o.OrderStatus,
                TotalAmount = o.TotalAmount,
                RestaurantID = o.RestaurantID,
                RestaurantName = o.Restaurant.RestaurantName,
                Items = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    MenuItemID = oi.MenuItemID,
                    MenuItemName = oi.MenuItem?.Name,
                    Price = oi.PriceAtOrder,
                    Quantity = oi.Quantity,
                }).ToList()
            });

            return new PaginatedResponseDTO<OrderResponseDTO>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        #endregion

        #region GetOrderHistoryAsync
        public async Task<PaginatedResponseDTO<OrderResponseDTO>> GetOrderHistoryAsync(int restaurantId, int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.OrderTransactions
                .Where(o => o.RestaurantID == restaurantId &&
                    o.DeliveryStatuses.Any(ds => ds.Status == "Delivered" || ds.Status == "Cancelled"))
                .Include(o => o.OrderItems).ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Restaurant)
                .Include(o => o.DeliveryStatuses);

            var totalCount = await query.CountAsync();
            if (totalCount == 0)
                throw new NoEntriessInCollectionException();

            var orders = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = orders.Select(o => new OrderResponseDTO
            {
                OrderID = o.OrderID,
                CreatedOn = o.CreatedOn,
                Status = o.OrderStatus,
                TotalAmount = o.TotalAmount,
                RestaurantID = o.RestaurantID,
                RestaurantName = o.Restaurant?.RestaurantName ?? string.Empty,
                Items = o.OrderItems.Select(oi => new OrderItemDTO
                {
                    MenuItemID = oi.MenuItemID,
                    MenuItemName = oi.MenuItem?.Name ?? string.Empty,
                    Price = oi.PriceAtOrder,
                    Quantity = oi.Quantity
                }).ToList(),
                DeliveryStatuses = o.DeliveryStatuses?
                    .OrderBy(ds => ds.UpdatedAt)
                    .Select(ds => new DeliveryStatusDTO
                    {
                        Status = ds.Status,
                        UpdatedAt = ds.UpdatedAt,
                        StatusUpdatedBy = ds.StatusUpdatedBy
                    }).ToList() ?? new List<DeliveryStatusDTO>()
            });

            return new PaginatedResponseDTO<OrderResponseDTO>
            {
                Items = result,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
        #endregion



        #region UpdateOrderStatusAsync
        public async Task<OrderResponseDTO?> UpdateOrderStatusAsync(int restaurantId, int orderId, string newStatus, int statusUpdatedBy)
        {
            var order = await _context.OrderTransactions
                .Include(o => o.DeliveryStatuses) // ✅ ensure we always load delivery statuses
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Restaurant)
                .FirstOrDefaultAsync(o => o.OrderID == orderId && o.RestaurantID == restaurantId);

            if (order == null)
                throw new NoSuchEntityException();

            // Update order status
            order.OrderStatus = newStatus;

            // Add new delivery status record
            var deliveryStatus = new DeliveryStatusTransaction
            {
                OrderID = orderId,
                Status = newStatus,
                UpdatedAt = DateTime.UtcNow,
                StatusUpdatedBy = statusUpdatedBy
            };

            if (order.DeliveryStatuses == null)
                order.DeliveryStatuses = new List<DeliveryStatusTransaction>();

            order.DeliveryStatuses.Add(deliveryStatus);
            _context.DeliveryStatusTransactions.Add(deliveryStatus);

            await _context.SaveChangesAsync();

            // Return updated DTO
            return new OrderResponseDTO
            {
                OrderID = order.OrderID,
                CreatedOn = order.CreatedOn,
                Status = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                RestaurantID = order.RestaurantID,
                RestaurantName = order.Restaurant?.RestaurantName ?? string.Empty,
                Items = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    MenuItemID = oi.MenuItemID,
                    MenuItemName = oi.MenuItem?.Name ?? string.Empty,
                    Price = oi.PriceAtOrder,
                    Quantity = oi.Quantity,
                   
                }).ToList(),
                DeliveryStatuses = order.DeliveryStatuses
                    .OrderBy(ds => ds.UpdatedAt)
                    .Select(ds => new DeliveryStatusDTO
                    {
                        Status = ds.Status,
                        UpdatedAt = ds.UpdatedAt,
                        StatusUpdatedBy = ds.StatusUpdatedBy
                    }).ToList()
            };
        }
        #endregion

        public async Task<IEnumerable<ReviewResponseDTO>> GetAllReviewsForRestaurantAsync(int restaurantId)
        {
            var reviews = await _context.ReviewTransactions
                .Include(r => r.User)
                .Include(r => r.MenuItem)
                .Where(r => r.MenuItem.RestaurantID == restaurantId)
                .OrderByDescending(r => r.CreatedOn)
                .Select(r => new ReviewResponseDTO
                {
                    ReviewID = r.ReviewID,
                    UserID = r.UserID,
                    UserName = r.User != null ? r.User.Username : string.Empty,
                    MenuItemID = r.MenuItemID,
                    MenuItemName = r.MenuItem != null ? r.MenuItem.Name : string.Empty,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedOn = r.CreatedOn
                })
                .ToListAsync();

            if (!reviews.Any())
                throw new NoEntriessInCollectionException();

            return reviews;
        }
    }
}

