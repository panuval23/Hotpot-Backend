using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class OrderItemDetails
    {
        public int OrderItemID { get; set; } 

        [Required(ErrorMessage = "OrderID is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "MenuItemID is required")]
        public int MenuItemID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price at order is required")]
        [Range(0.0, double.MaxValue, ErrorMessage = "Price at order must be a positive value")]
        public decimal PriceAtOrder { get; set; }

        // Navigation properties
        public OrderTransaction? Order { get; set; }
        public MenuItemMaster? MenuItem { get; set; }
    }
}
