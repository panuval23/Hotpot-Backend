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
    [Authorize(Roles = "Restaurant")]
    [EnableCors("DefaultCORS")]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ILogger<RestaurantController> _logger;
        private readonly IWebHostEnvironment _env;

        public RestaurantController(IRestaurantService restaurantService, ILogger<RestaurantController> logger, IWebHostEnvironment env)
        {
            _restaurantService = restaurantService;
            _logger = logger;
            _env = env;
        }

        private int GetLoggedInRestaurantId()
        {
            var restaurantIdClaim = User.FindFirst("RestaurantId")?.Value;
            if (string.IsNullOrEmpty(restaurantIdClaim) || !int.TryParse(restaurantIdClaim, out int restaurantId))
            {
                throw new UnauthorizedAccessException("Invalid RestaurantId claim.");
            }
            return restaurantId;
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

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var categories = await _restaurantService.GetCategoriesByRestaurantAsync(restaurantId);
                return Ok(categories);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to GetCategories");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return StatusCode(500, new { Message = $"Error fetching categories: {ex.Message}" });
            }
        }

        [HttpPost("AddCategory")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryCreateDTO dto)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var category = await _restaurantService.AddCategoryAsync(restaurantId, dto);
                if (category == null)
                    return BadRequest(new { Message = "Category already exists or invalid data." });

                return CreatedAtAction(nameof(GetCategories), category);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to AddCategory");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding category");
                return StatusCode(500, new { Message = $"Error adding category: {ex.Message}" });
            }
        }

        [HttpPut("UpdateCategory/{categoryId}")]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryUpdateDTO dto)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var updated = await _restaurantService.UpdateCategoryAsync(restaurantId, categoryId, dto);
                if (updated == null)
                    return NotFound(new { Message = $"Category with ID {categoryId} not found in your restaurant." });

                return Ok(updated);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to UpdateCategory");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return StatusCode(500, new { Message = $"Error updating category: {ex.Message}" });
            }
        }

        [HttpDelete("DeleteCategory/{categoryId}")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var success = await _restaurantService.DeleteCategoryAsync(restaurantId, categoryId);
                if (!success)
                    return NotFound(new { Message = "Category not found in your restaurant." });

                return Ok(new { Message = "Category deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to DeleteCategory");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return StatusCode(500, new { Message = $"Error deleting category: {ex.Message}" });
            }
        }

        [HttpGet("GetAllMenuItems")]
        public async Task<IActionResult> GetAllMenuItems(
       [FromQuery] int pageNumber = 1,
       [FromQuery] int pageSize = 10)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var items = await _restaurantService.GetAllMenuItemsAsync(restaurantId, pageNumber, pageSize);
                return Ok(items);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to GetAllMenuItems");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching menu items");
                return StatusCode(500, new { Message = $"Error fetching menu items: {ex.Message}" });
            }
        }


        [HttpGet("Getmenuitems/{menuItemId}")]
        public async Task<IActionResult> GetMenuItemById(int menuItemId)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var menuItem = await _restaurantService.GetMenuItemByIdAsync(restaurantId, menuItemId);
                if (menuItem == null)
                    return NotFound(new { Message = $"Menu item with ID {menuItemId} not found." });

                return Ok(menuItem);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to GetMenuItemById");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching menu item");
                return StatusCode(500, new { Message = $"Error fetching menu item: {ex.Message}" });
            }
        }

        [HttpPost("AddMenuItem")]
        public async Task<IActionResult> AddMenuItem([FromBody] MenuItemCreateDTO dto)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var menuItem = await _restaurantService.AddMenuItemAsync(restaurantId, dto);
                if (menuItem == null)
                    return BadRequest(new { Message = "Invalid data or category does not belong to your restaurant." });

                return CreatedAtAction(nameof(GetMenuItemById), new { menuItemId = menuItem.MenuItemID }, menuItem);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to AddMenuItem");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding menu item");
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new { Message = $"Error adding menu item: {innerMessage}" });
            }
        }

        [HttpPut("Updatemenuitems/{menuItemId}")]
        public async Task<IActionResult> UpdateMenuItem(int menuItemId, [FromBody] MenuItemUpdateDTO dto)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var updatedMenuItem = await _restaurantService.UpdateMenuItemAsync(restaurantId, menuItemId, dto);
                if (updatedMenuItem == null)
                    return NotFound(new { Message = $"Menu item with ID {menuItemId} not found or invalid category." });

                return Ok(updatedMenuItem);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to UpdateMenuItem");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating menu item");
                return StatusCode(500, new { Message = $"Error updating menu item: {ex.Message}" });
            }
        }

        [HttpDelete("Deletemenuitems/{menuItemId}")]
        public async Task<IActionResult> DeleteMenuItem(int menuItemId)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var success = await _restaurantService.DeleteMenuItemAsync(restaurantId, menuItemId);
                if (!success)
                    return NotFound(new { Message = $"Menu item with ID {menuItemId} not found in your restaurant." });

                return Ok(new { Message = "Menu item deleted successfully." });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to DeleteMenuItem");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting menu item");
                return StatusCode(500, new { Message = $"Error deleting menu item: {ex.Message}" });
            }
        }

        [HttpPost("discounts/add")]
        public async Task<IActionResult> AddDiscount([FromBody] AddDiscountDTO discountDto)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var menuItem = await _restaurantService.GetMenuItemByIdAsync(restaurantId, discountDto.MenuItemID);
                if (menuItem == null)
                    return BadRequest(new { Message = "Invalid MenuItemID for this restaurant." });

                await _restaurantService.AddDiscountAsync(restaurantId, discountDto);
                return Ok(new { Message = "Discount added successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to AddDiscount");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding discount");
                return StatusCode(500, new { Message = $"Error adding discount: {ex.Message}" });
            }
        }

        [HttpGet("orders/current")]
        public async Task<IActionResult> GetCurrentOrders()
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var currentOrders = await _restaurantService.GetCurrentOrdersAsync(restaurantId);
                return Ok(currentOrders);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to GetCurrentOrders");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching current orders");
                return StatusCode(500, new { Message = $"Error fetching current orders: {ex.Message}" });
            }
        }

        [HttpPut("orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusDTO request)
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                int statusUpdatedBy = GetLoggedInUserId();

                var updatedOrder = await _restaurantService.UpdateOrderStatusAsync(restaurantId, orderId, request.NewStatus, statusUpdatedBy);
                if (updatedOrder == null)
                    return NotFound(new { Message = $"Order with ID {orderId} not found for your restaurant." });

                return Ok(new { Message = "Order status updated successfully", updatedOrder });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to UpdateOrderStatus");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order status");
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("orders/history")]
        public async Task<IActionResult> GetOrderHistory()
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var orders = await _restaurantService.GetOrderHistoryAsync(restaurantId);
                return Ok(orders);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to GetOrderHistory");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching order history");
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        [HttpGet("reviews")]
        public async Task<IActionResult> GetAllReviews()
        {
            try
            {
                int restaurantId = GetLoggedInRestaurantId();
                var reviews = await _restaurantService.GetAllReviewsForRestaurantAsync(restaurantId);
                return Ok(reviews);
            }
            catch (NoEntriessInCollectionException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching reviews for restaurant");
                return StatusCode(500, new { Message = $"Error fetching reviews: {ex.Message}" });
            }
        }

        [HttpGet("images")]
        public IActionResult GetRestaurantImages()
        {
            try
            {
                var restaurantId = GetLoggedInRestaurantId();
                var uploadsPath = Path.Combine(_env.WebRootPath, "uploads", $"Restaurant_{restaurantId}");

                if (!Directory.Exists(uploadsPath))
                    return Ok(new string[0]);

                var files = Directory.GetFiles(uploadsPath)
                                     .Select(f => Path.GetFileName(f))
                                     .ToList();

                var baseUrl = $"{Request.Scheme}://{Request.Host}/uploads/Restaurant_{restaurantId}/";
                var urls = files.Select(f => baseUrl + Uri.EscapeDataString(f)).ToList();

                return Ok(urls);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to GetRestaurantImages");
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching restaurant images");
                return StatusCode(500, new { Message = $"Error fetching images: {ex.Message}" });
            }
        }


    }
}

