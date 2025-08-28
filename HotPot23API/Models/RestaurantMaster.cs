using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class RestaurantMaster
    {
        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "UserID is required")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Restaurant name is required")]
        [StringLength(100, ErrorMessage = "Restaurant name cannot be longer than 100 characters")]
        public string RestaurantName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Cuisine type cannot be longer than 50 characters")]
        public string? CuisineType { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot be longer than 200 characters")]
        public string? Location { get; set; }

        [Range(1, 240, ErrorMessage = "Average preparation time should be between 1 and 240 minutes")]
        public int? AveragePreparationTime { get; set; }

        [StringLength(300, ErrorMessage = "Image URL cannot be longer than 300 characters")]
        public string? ImageUrl { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        // Navigation properties
        public UserMaster? User { get; set; }
        public ICollection<MenuItemMaster>? MenuItems { get; set; }
        public ICollection<OrderTransaction>? Orders { get; set; }
        public ICollection<CategoryMaster>? Categories { get; set; }
    }
}
