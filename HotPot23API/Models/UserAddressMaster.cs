using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class UserAddressMaster
    {
        public int AddressID { get; set; }  

        public int UserID { get; set; }  

        [Required(ErrorMessage = "Address line is required.")]
        [StringLength(200, ErrorMessage = "Address line cannot exceed 200 characters.")]
        public string AddressLine { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pincode is required.")]
        [StringLength(10, ErrorMessage = "Pincode cannot exceed 10 characters.")]
        public string Pincode { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Landmark cannot exceed 100 characters.")]
        public string Landmark { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address type is required.")]
        [StringLength(20, ErrorMessage = "Address type cannot exceed 20 characters.")]
        public string AddressType { get; set; } = string.Empty;  

        public bool IsDefault { get; set; }

        public DateTime CreatedOn { get; set; }

        public UserMaster User { get; set; } = null!;
        public ICollection<OrderTransaction> Orders { get; set; } = new List<OrderTransaction>();
    }
}
