using HotPot23API.Exceptions;
using HotPot23API.Interfaces;
using HotPot23API.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotPot23API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    [EnableCors("DefaultCORS")]


    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        private int GetLoggedInUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid UserId claim.");
            }

            return userId;
        }

        [HttpGet("GetAllRestaurants")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetAllRestaurants([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var restaurants = await _userService.GetAllRestaurantsAsync(pageNumber, pageSize);
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all restaurants");
                return StatusCode(500, new { Message = $"Error fetching restaurants: {ex.Message}" });
            }
        }

        [HttpGet("menu")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetMenu(
      string restaurantName = null,
      string categoryName = null,
      bool? isVeg = null,
      decimal? minPrice = null,
      decimal? maxPrice = null,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
        {
            try
            {
                var menu = await _userService.GetMenuByRestaurantAsync(
                    restaurantName, categoryName, isVeg, minPrice, maxPrice, pageNumber, pageSize);

                return Ok(menu);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching menu");
                return StatusCode(500, new { Message = $"Error fetching menu: {ex.Message}" });
            }
        }

        [HttpGet("menu/search")]
        //[AllowAnonymous]
        public async Task<IActionResult> SearchMenuItems(
            [FromQuery] string searchTerm,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var results = await _userService.SearchMenuItemsAsync(searchTerm, pageNumber, pageSize);
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching menu items");
                return StatusCode(500, new { Message = $"Error searching menu items: {ex.Message}" });
            }
        }


        [HttpGet("menu/{menuItemId}")]
       // [AllowAnonymous]
        public async Task<IActionResult> GetMenuItemDetails(int menuItemId)
        {
            try
            {
                var menuItem = await _userService.GetMenuItemDetailsAsync(menuItemId);
                if (menuItem == null)
                    return NotFound(new { Message = "Menu item not found." });

                return Ok(menuItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching menu item details for id {menuItemId}");
                return StatusCode(500, new { Message = $"Error fetching menu item: {ex.Message}" });
            }
        }

        [HttpPost("cart/add")]
        //[Authorize(Roles = "User")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int userId = GetLoggedInUserId();

                var result = await _userService.AddToCartAsync(userId, request);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to AddToCart");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in AddToCart");
                return BadRequest(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument in AddToCart");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding to cart");
                return StatusCode(500, new { Message = $"Error adding to cart: {ex.Message}" });
            }
        }

        [HttpGet("cart")]
       // [Authorize(Roles = "User")]
        public async Task<IActionResult> GetCart()
        {
            try
            {
                int userId = GetLoggedInUserId();
                var cart = await _userService.GetCartAsync(userId);
                return Ok(cart);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to GetCart");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cart");
                return StatusCode(500, new { Message = $"Error fetching cart: {ex.Message}" });
            }
        }

        [HttpPut("cart/update")]
        //[Authorize(Roles = "User")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDTO updateCartItemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int userId = GetLoggedInUserId();
                var updatedItem = await _userService.UpdateCartItemAsync(userId, updateCartItemDto);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating cart item");
                return StatusCode(500, new { Message = $"Error updating cart item: {ex.Message}" });
            }
        }

        [HttpDelete("cart/remove/{cartId}")]
       // [Authorize(Roles = "User")]
        public async Task<IActionResult> RemoveCartItem(int cartId)
        {
            try
            {
                int userId = GetLoggedInUserId();
                bool success = await _userService.RemoveCartItemAsync(userId, cartId);
                if (!success)
                    return NotFound(new { Message = "Cart item not found or you don't have permission." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cart item");
                return StatusCode(500, new { Message = $"Error removing cart item: {ex.Message}" });
            }
        }

        [HttpPost("cart/checkout")]
       // [Authorize(Roles = "User")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDTO checkoutDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int userId = GetLoggedInUserId();
                var orderResponse = await _userService.CheckoutAsync(userId, checkoutDto);
                return Ok(new { Message = "Order placed successfully", Order = orderResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout");
                return StatusCode(500, new { Message = $"Error during checkout: {ex.GetBaseException().Message}" });
            }
        }

        //[HttpGet("restaurants-by-menu")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetRestaurantsByMenu([FromQuery] string menuName)
        //{
        //    try
        //    {
        //        var restaurants = await _userService.GetRestaurantsByMenuAsync(menuName);
        //        return Ok(restaurants);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error fetching restaurants for menu {menuName}");
        //        return StatusCode(500, new { Message = $"Error fetching restaurants: {ex.Message}" });
        //    }
        //}

        [HttpPost("review/add")]
        //[Authorize(Roles = "User")]
        public async Task<IActionResult> AddReview([FromBody] AddReviewDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                int userId = GetLoggedInUserId();
                var message = await _userService.AddReviewAsync(userId, request);
                return Ok(new { Message = message });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to AddReview");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (NoSuchEntityException ex)
            {
                _logger.LogWarning(ex, "Menu item not found");
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid rating in AddReview");
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                return StatusCode(500, new { Message = $"Error adding review: {ex.Message}" });
            }
        }
    }
}
