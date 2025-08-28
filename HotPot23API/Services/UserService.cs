using AutoMapper;
using HotPot23API.Contexts;
using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models;
using HotPot23API.Models.DTOs;
using HotPot23API.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotPot23API.Services
{

    public class UserService : IUserService
    {
        private readonly FoodDeliveryManagementContext _context;
        private readonly IMapper _mapper;

        public UserService(FoodDeliveryManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PaginatedResponseDTO<UserRestaurantDTO>> GetAllRestaurantsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var query = _context.RestaurantMasters
                .Include(r => r.MenuItems);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var restaurants = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Map to DTOs
            var restaurantDtos = _mapper.Map<IEnumerable<UserRestaurantDTO>>(restaurants);

            // Return in paginated format
            return new PaginatedResponseDTO<UserRestaurantDTO>
            {
                Items = restaurantDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<PaginatedResponseDTO<UserMenuItemResponseDTO>> GetMenuByRestaurantAsync(
       string restaurantName = null,
       string categoryName = null,
       bool? isVeg = null,
       decimal? minPrice = null,
       decimal? maxPrice = null,
       int pageNumber = 1,
       int pageSize = 10)
        {
            var query = _context.MenuItems
                 .Where(m => m.IsActive)
                .Include(m => m.Category)
                .Include(m => m.Restaurant)
                .Include(m => m.Discounts)
                .AsQueryable();

            if (!string.IsNullOrEmpty(restaurantName))
            {
                var restaurant = await _context.RestaurantMasters
                    .FirstOrDefaultAsync(r => r.RestaurantName.ToLower() == restaurantName.ToLower());

                if (restaurant != null)
                    query = query.Where(m => m.RestaurantID == restaurant.RestaurantID);
                else
                    return new PaginatedResponseDTO<UserMenuItemResponseDTO>
                    {
                        Items = Enumerable.Empty<UserMenuItemResponseDTO>(),
                        TotalCount = 0,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                var category = await _context.CategoryMasters
                    .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == categoryName.ToLower());

                if (category != null)
                    query = query.Where(m => m.CategoryID == category.CategoryID);
                else
                    return new PaginatedResponseDTO<UserMenuItemResponseDTO>
                    {
                        Items = Enumerable.Empty<UserMenuItemResponseDTO>(),
                        TotalCount = 0,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
            }

            if (isVeg.HasValue)
                query = query.Where(m => m.IsVeg == isVeg.Value);

            if (minPrice.HasValue)
                query = query.Where(m => m.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(m => m.Price <= maxPrice.Value);

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var pagedQuery = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var menuItems = await pagedQuery.ToListAsync();

            var menuItemDtos = _mapper.Map<IEnumerable<UserMenuItemResponseDTO>>(menuItems);

            return new PaginatedResponseDTO<UserMenuItemResponseDTO>
            {
                Items = menuItemDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task<PaginatedResponseDTO<UserMenuItemResponseDTO>> SearchMenuItemsAsync(
    string searchTerm, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentNullException(nameof(searchTerm), "Search term cannot be null or empty.");

            var query = _context.MenuItems
                .Where(m => m.IsActive &&
                            (m.Name.Contains(searchTerm) || m.Description.Contains(searchTerm))) // ✅ Keep search condition
                .Include(m => m.Category)
                .Include(m => m.Restaurant)
                .Include(m => m.Discounts)
                .AsQueryable();

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var pagedQuery = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var menuItems = await pagedQuery.ToListAsync();

            var menuItemDtos = _mapper.Map<IEnumerable<UserMenuItemResponseDTO>>(menuItems);

            return new PaginatedResponseDTO<UserMenuItemResponseDTO>
            {
                Items = menuItemDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }



        public async Task<UserMenuItemResponseDTO> GetMenuItemDetailsAsync(int menuItemId)
        {
            var menuItem = await _context.MenuItems
                 .Where(m => m.IsActive)
                .Include(m => m.Category)
                  .Include(m => m.Discounts)
                .Include(m => m.Restaurant)
                .FirstOrDefaultAsync(m => m.MenuItemID == menuItemId);
           

            return _mapper.Map<UserMenuItemResponseDTO>(menuItem);
        }


  
        public async Task<CartItemDTO> AddToCartAsync(int userId, AddToCartDTO addToCartDto)
        {
            if (addToCartDto == null)
                throw new ArgumentNullException(nameof(addToCartDto));

            if (addToCartDto.Quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1", nameof(addToCartDto.Quantity));

         
            var menuItem = await _context.MenuItems
                .Where(m => m.MenuItemID == addToCartDto.MenuItemID && m.RestaurantID == addToCartDto.RestaurantID)
                .Include(m => m.Discounts)
                .FirstOrDefaultAsync();

            if (menuItem == null)
                throw new Exception("Menu item not found.");

         
            var existingCart = await _context.CartTransactions
                .FirstOrDefaultAsync(c => c.UserID == userId && c.MenuItemID == addToCartDto.MenuItemID);

            if (existingCart != null)
            {
                existingCart.Quantity += addToCartDto.Quantity;
                _context.CartTransactions.Update(existingCart);
            }
            else
            {
                existingCart = new CartTransaction
                {
                    UserID = userId,
                    MenuItemID = addToCartDto.MenuItemID,
                    Quantity = addToCartDto.Quantity,
                    AddedOn = DateTime.UtcNow
                };

                await _context.CartTransactions.AddAsync(existingCart);
            }

            await _context.SaveChangesAsync();

           
            decimal? discountPercent = null;
            var now = DateTime.UtcNow;
            if (menuItem.Discounts != null && menuItem.Discounts.Any())
            {
                var activeDiscount = menuItem.Discounts
                    .Where(d => (!d.ValidFrom.HasValue || d.ValidFrom.Value <= now)
                             && (!d.ValidTo.HasValue || d.ValidTo.Value >= now))
                    .OrderByDescending(d => d.DiscountPercent ?? 0)   
                    .FirstOrDefault();

                if (activeDiscount != null)
                    discountPercent = activeDiscount.DiscountPercent;
            }

            return new CartItemDTO
            {
                CartID = existingCart.CartID,
                MenuItemID = menuItem.MenuItemID,
                RestaurantID = menuItem.RestaurantID,
                MenuItemName = menuItem.Name,
                OriginalPrice = menuItem.Price,
                DiscountPercent = discountPercent,
                Quantity = existingCart.Quantity
            };
        }

        public async Task<CartResponseDTO> GetCartAsync(int userId)
        {
            var cartItems = await _context.CartTransactions
                .Where(c => c.UserID == userId)
                .Include(c => c.MenuItem)
                    .ThenInclude(m => m.Discounts)
                .ToListAsync();

            var now = DateTime.UtcNow;

            var cartItemDtos = cartItems.Select(c =>
            {
                var menuItem = c.MenuItem;

             
                decimal? discountPercent = null;
                if (menuItem.Discounts != null && menuItem.Discounts.Any())
                {
                    var activeDiscount = menuItem.Discounts
                        .Where(d => (!d.ValidFrom.HasValue || d.ValidFrom <= now) &&
                                    (!d.ValidTo.HasValue || d.ValidTo >= now))
                        .OrderByDescending(d => d.DiscountPercent ?? 0)
                        .FirstOrDefault();

                    if (activeDiscount != null)
                        discountPercent = activeDiscount.DiscountPercent;
                }

                return new CartItemDTO
                {
                    CartID = c.CartID,
                    MenuItemID = menuItem.MenuItemID,
                    MenuItemName = menuItem.Name,
                    OriginalPrice = menuItem.Price,
                    DiscountPercent = discountPercent,
                    Quantity = c.Quantity
                };
            }).ToList();

            return new CartResponseDTO
            {
                Items = cartItemDtos
            };
        }
        public async Task<CartItemDTO> UpdateCartItemAsync(int userId, UpdateCartItemDTO updateCartItemDto)
        {
            if (updateCartItemDto == null)
                throw new ArgumentNullException(nameof(updateCartItemDto));

            if (updateCartItemDto.Quantity <= 0)
                throw new ArgumentException("Quantity must be at least 1", nameof(updateCartItemDto.Quantity));

   
            var cartItem = await _context.CartTransactions
                .Include(c => c.MenuItem)
                .ThenInclude(m => m.Discounts)
                .FirstOrDefaultAsync(c => c.CartID == updateCartItemDto.CartID && c.UserID == userId);

            if (cartItem == null)
                throw new KeyNotFoundException("Cart item not found.");

        
            cartItem.Quantity = updateCartItemDto.Quantity;

            _context.CartTransactions.Update(cartItem);
            await _context.SaveChangesAsync();

     
            decimal? discountPercent = null;
            var now = DateTime.UtcNow;
            if (cartItem.MenuItem.Discounts != null && cartItem.MenuItem.Discounts.Any())
            {
                var activeDiscount = cartItem.MenuItem.Discounts
                    .Where(d => (!d.ValidFrom.HasValue || d.ValidFrom.Value <= now)
                             && (!d.ValidTo.HasValue || d.ValidTo.Value >= now))
                    .OrderByDescending(d => d.DiscountPercent ?? 0)
                    .FirstOrDefault();

                if (activeDiscount != null)
                    discountPercent = activeDiscount.DiscountPercent;
            }

      
            return new CartItemDTO
            {
                CartID = cartItem.CartID,
                MenuItemID = cartItem.MenuItemID,
                MenuItemName = cartItem.MenuItem.Name,
                OriginalPrice = cartItem.MenuItem.Price,
                DiscountPercent = discountPercent,
                Quantity = cartItem.Quantity
            };
        }

        public async Task<bool> RemoveCartItemAsync(int userId, int cartId)
        {
            var cartItem = await _context.CartTransactions
                .FirstOrDefaultAsync(c => c.CartID == cartId && c.UserID == userId);

            if (cartItem == null)
                return false;

            _context.CartTransactions.Remove(cartItem);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<OrderResponseDTO> CheckoutAsync(int userId, CheckoutDTO request)
        {
           
            var cartItems = await _context.CartTransactions
                .Where(c => c.UserID == userId)
                .Include(c => c.MenuItem)
                    .ThenInclude(m => m.Discounts)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
                throw new Exception("Cart is empty.");

            
            var restaurantId = cartItems.First().MenuItem.RestaurantID;

      
            var order = new OrderTransaction
            {
                UserID = userId,
                RestaurantID = restaurantId,
                ShippingAddressID = request.ShippingAddressID,
                OrderStatus = "Pending",
                PaymentMethod = request.PaymentMethod,
                CreatedOn = DateTime.UtcNow,
                OrderItems = new List<OrderItemDetails>()
            };

            decimal totalAmount = 0m;
            var now = DateTime.UtcNow;

          
            foreach (var cartItem in cartItems)
            {
                var menuItem = cartItem.MenuItem;

                decimal discountPercent = 0m;
                var activeDiscount = menuItem.Discounts?
                    .Where(d => (!d.ValidFrom.HasValue || d.ValidFrom.Value <= now)
                             && (!d.ValidTo.HasValue || d.ValidTo.Value >= now))
                    .OrderByDescending(d => d.DiscountPercent ?? 0)
                    .FirstOrDefault();

                if (activeDiscount != null)
                    discountPercent = activeDiscount.DiscountPercent ?? 0m;

                decimal discountedPrice = menuItem.Price * (1 - discountPercent / 100);

                totalAmount += discountedPrice * cartItem.Quantity;

                order.OrderItems.Add(new OrderItemDetails
                {
                    MenuItemID = menuItem.MenuItemID,
                    Quantity = cartItem.Quantity,
                    PriceAtOrder = discountedPrice
                });
            }

            order.TotalAmount = totalAmount;

            _context.OrderTransactions.Add(order);
            _context.CartTransactions.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            var savedOrder = await _context.OrderTransactions
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.OrderID == order.OrderID);

            if (savedOrder == null)
                throw new Exception("Failed to retrieve saved order.");

            var response = new OrderResponseDTO
            {
                OrderID = savedOrder.OrderID,
                CreatedOn = savedOrder.CreatedOn,
                Status = savedOrder.OrderStatus,
                TotalAmount = savedOrder.TotalAmount,
                RestaurantID = savedOrder.RestaurantID,
                RestaurantName = savedOrder.Restaurant?.RestaurantName,
                Items = savedOrder.OrderItems.Select(oi => new OrderItemDTO
                {
                    MenuItemID = oi.MenuItemID,
                    MenuItemName = oi.MenuItem?.Name,
                    Price = oi.PriceAtOrder,
                    Quantity = oi.Quantity,
                }).ToList()
            };

            return response;

        }
        //public async Task<IEnumerable<UserRestaurantDTO>> GetRestaurantsByMenuAsync(string menuName)
        //{
        //    if (string.IsNullOrWhiteSpace(menuName))
        //        throw new ArgumentNullException(nameof(menuName), "Menu name cannot be null or empty.");

        //    var restaurants = await _context.RestaurantMasters
        //        .Include(r => r.MenuItems)
        //        .Where(r => r.MenuItems.Any(m => m.IsActive && m.Name.ToLower().Contains(menuName.ToLower())))
        //        .ToListAsync();

        //    return _mapper.Map<IEnumerable<UserRestaurantDTO>>(restaurants);
        //}


        public async Task<string> AddReviewAsync(int userId, AddReviewDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var menuItem = await _context.MenuItems
                .Include(m => m.Restaurant)
                .FirstOrDefaultAsync(m => m.MenuItemID == dto.MenuItemID);

            if (menuItem == null)
                throw new NoSuchEntityException();

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var review = new ReviewTransaction
            {
                UserID = userId,
                MenuItemID = dto.MenuItemID,
                Rating = dto.Rating,
                Comment = dto.Comment ?? string.Empty,
                CreatedOn = DateTime.UtcNow
            };

            _context.ReviewTransactions.Add(review);
            await _context.SaveChangesAsync();

            return $"Review added successfully for {menuItem.Name} at {menuItem.Restaurant.RestaurantName}";
        }

    }

}
