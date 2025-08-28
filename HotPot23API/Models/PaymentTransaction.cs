using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class PaymentTransaction
    {
        public int PaymentID { get; set; } // Primary key handled via Fluent API

        [Required(ErrorMessage = "OrderID is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Amount paid is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount paid must be greater than zero")]
        public decimal AmountPaid { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        [StringLength(50, ErrorMessage = "Payment method cannot be longer than 50 characters")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment status is required")]
        [StringLength(50, ErrorMessage = "Payment status cannot be longer than 50 characters")]
        public string PaymentStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Transaction time is required")]
        public DateTime TransactionTime { get; set; }

        // Navigation property
        public OrderTransaction? Order { get; set; }
    }
}
