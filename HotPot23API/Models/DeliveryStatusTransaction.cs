using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class DeliveryStatusTransaction
    {
        public int StatusID { get; set; } 

        [Required(ErrorMessage = "OrderID is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(100, ErrorMessage = "Status cannot be longer than 100 characters")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "UpdatedAt datetime is required")]
        public DateTime UpdatedAt { get; set; }

        [Required(ErrorMessage = "StatusUpdatedBy (UserID) is required")]
        public int StatusUpdatedBy { get; set; }

        // Navigation properties
        public OrderTransaction? Order { get; set; }
        public UserMaster? User { get; set; }
    }
}
