using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class CategoryMaster
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "RestaurantID is required")]
        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name cannot be longer than 100 characters")]
        public string CategoryName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public RestaurantMaster? Restaurant { get; set; }

        public ICollection<MenuItemMaster>? MenuItems { get; set; } = new List<MenuItemMaster>();
    }
}
