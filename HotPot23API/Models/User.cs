using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class User
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password hash is required.")]
        public byte[] Password { get; set; } = null!;

        [Required(ErrorMessage = "Hash key is required.")]
        public byte[] HashKey { get; set; } = null!;

        [Required(ErrorMessage = "Role is required.")]
        [StringLength(20, ErrorMessage = "Role cannot exceed 20 characters.")]
        public string Role { get; set; } = string.Empty;

        public int UserID { get; set; }

        public UserMaster UserMaster { get; set; } = null!;
    }
}
