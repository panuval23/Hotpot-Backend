using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class CartTransaction
    {
        public int CartID { get; set; }  

        [Required(ErrorMessage = "UserID is required")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "MenuItemID is required")]
        public int MenuItemID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public DateTime AddedOn { get; set; } = DateTime.UtcNow;


        // Navigation properties
        public UserMaster? User { get; set; }
        public MenuItemMaster? MenuItem { get; set; }
    }
}
