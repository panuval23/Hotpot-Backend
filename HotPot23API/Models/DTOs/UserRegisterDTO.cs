

namespace HotPot23API.Models.DTOs
{
    public class UserRegisterDTO
    {
   
        public string Username { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

    
        public string Email { get; set; } = string.Empty;


        public string Gender { get; set; } = string.Empty;

      
        public string ContactNumber { get; set; } = string.Empty;

    
        public string Role { get; set; } = "User";

        
        public string AddressLine { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

      
        public string State { get; set; } = string.Empty;


        public string Pincode { get; set; } = string.Empty;


        public string Landmark { get; set; } = string.Empty;

        
        public string AddressType { get; set; } = "Home";

        public bool IsDefault { get; set; } = true;
    }
}
