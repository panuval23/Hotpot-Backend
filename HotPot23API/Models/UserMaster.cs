using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class UserMaster
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password hash is required.")]
        [MinLength(60, ErrorMessage = "Password hash must be at least 60 characters.")]
        public string PasswordHash { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Invalid contact number.")]
        [StringLength(15, ErrorMessage = "Contact number cannot exceed 15 characters.")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters.")]
        public string Role { get; set; } = string.Empty;  // e.g. "Admin", "Restaurant", "User"

        public bool IsActive { get; set; }

        public DateTime CreatedOn { get; set; }

        // Navigation properties (no validation needed here)
        public ICollection<UserAddressMaster> Addresses { get; set; } = new List<UserAddressMaster>();
        public ICollection<RestaurantMaster> Restaurants { get; set; } = new List<RestaurantMaster>();
        public ICollection<CartTransaction> CartTransactions { get; set; } = new List<CartTransaction>();
        public ICollection<OrderTransaction> Orders { get; set; } = new List<OrderTransaction>();
        public ICollection<DeliveryStatusTransaction> DeliveryStatuses { get; set; } = new List<DeliveryStatusTransaction>();
        public ICollection<ReviewTransaction> Reviews { get; set; } = new List<ReviewTransaction>();
        public User? AuthUser { get; set; } // Add this property
    }
}
