using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class MenuItemMaster
    {
        public int MenuItemID { get; set; }

        [Required(ErrorMessage = "RestaurantID is required")]
        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "CategoryID is required")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [StringLength(500, ErrorMessage = "Description can't be longer than 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Availability time is required")]
        [StringLength(50, ErrorMessage = "Availability time can't be longer than 50 characters")]
        public string AvailabilityTime { get; set; } = string.Empty;

        public bool IsVeg { get; set; }

        [StringLength(50, ErrorMessage = "Taste info can't be longer than 50 characters")]
        public string TasteInfo { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Nutritional info can't be longer than 200 characters")]
        public string NutritionalInfo { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid image URL format")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool InStock { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public RestaurantMaster? Restaurant { get; set; }
        public CategoryMaster? Category { get; set; }
        public ICollection<DiscountMaster>? Discounts { get; set; }
        public ICollection<CartTransaction>? Carts { get; set; }
        public ICollection<OrderItemDetails>? OrderItems { get; set; }
        public ICollection<ReviewTransaction>? Reviews { get; set; }
    }
}
