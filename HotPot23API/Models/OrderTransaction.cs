using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class OrderTransaction
    {
        public int OrderID { get; set; }  

        [Required(ErrorMessage = "UserID is required")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "RestaurantID is required")]
        public int RestaurantID { get; set; }

        [Required(ErrorMessage = "Shipping AddressID is required")]
        public int ShippingAddressID { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be a positive value")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Order status is required")]
        [StringLength(50, ErrorMessage = "Order status cannot be longer than 50 characters")]
        public string OrderStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50, ErrorMessage = "Payment method cannot be longer than 50 characters")]
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public UserMaster? User { get; set; }
        public RestaurantMaster? Restaurant { get; set; }
        public UserAddressMaster? ShippingAddress { get; set; }
        public ICollection<OrderItemDetails>? OrderItems { get; set; }
        public PaymentTransaction? Payment { get; set; }
        public ICollection<DeliveryStatusTransaction>? DeliveryStatuses { get; set; }
    }
}
