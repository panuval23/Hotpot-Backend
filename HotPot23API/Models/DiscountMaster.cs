using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class DiscountMaster
    {
        public int DiscountID { get; set; }  

        [Required(ErrorMessage = "MenuItemID is required")]
        public int MenuItemID { get; set; }

        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100")]
        public decimal? DiscountPercent { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation property
        public MenuItemMaster? MenuItem { get; set; }
    }
}
