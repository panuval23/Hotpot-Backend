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
    [Authorize(Roles = "Admin")]
    [EnableCors("DefaultCORS")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IAdminService adminService, ILogger<AdminController> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        [HttpPost("AddRestaurant")]
        public async Task<IActionResult> AddRestaurant([FromBody] AdminRestaurantRegisterDTO dto)
        {
            try
            {
                var response = await _adminService.AddRestaurantAsync(dto);
                if (response == null)
                    return BadRequest(new { Message = "Invalid data or user role." });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding restaurant");
                return StatusCode(500, new { Message = $"Error adding restaurant: {ex.Message}" });
            }
        }

        [HttpPut("UpdateRestaurant/{restaurantId}")]
        public async Task<IActionResult> UpdateRestaurant(int restaurantId, [FromBody] AdminRestaurantRegisterDTO dto)
        {
            try
            {
                var response = await _adminService.UpdateRestaurantAsync(restaurantId, dto);
                if (response == null)
                    return NotFound(new { Message = $"Restaurant with ID {restaurantId} not found." });
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating restaurant with ID {RestaurantId}", restaurantId);
                return StatusCode(500, new { Message = $"Error updating restaurant: {ex.Message}" });
            }
        }

        [HttpDelete("DeleteRestaurant/{restaurantId}")]
        public async Task<IActionResult> DeleteRestaurant(int restaurantId)
        {
            try
            {
                var success = await _adminService.DeleteRestaurantAsync(restaurantId);
                if (!success)
                    return NotFound(new { Message = "Restaurant not found." });
                return Ok(new { Message = "Restaurant deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting restaurant with ID {RestaurantId}", restaurantId);
                return StatusCode(500, new { Message = $"Error deleting restaurant: {ex.Message}" });
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _adminService.GetAllUsersAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                return StatusCode(500, "Error fetching users.");
            }
        }

        [HttpGet("restaurants")]
        public async Task<IActionResult> GetRestaurants(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var result = await _adminService.GetAllRestaurantsAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching restaurants - Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
                return StatusCode(500, "Error fetching restaurants.");
            }
        }
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var success = await _adminService.DeleteUserAsync(userId);
                if (!success)
                    return NotFound(new { Message = "User not found." });

                return Ok(new { Message = "User deactivated (soft delete)." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {UserId}", userId);
                return StatusCode(500, new { Message = $"Error deleting user: {ex.Message}" });
            }
        }



    }
}
