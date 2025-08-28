namespace HotPot23API.Models.DTOs
{
    public class AdminRestaurantRegisterDTO
    {
        public int? UserId { get; set; }

        // Mode 2: New user
        public string? Username { get; set; }
        public string? OwnerName { get; set; }
        public string? Email { get; set; }
        public string? Gender { get; set; }
        public string? ContactNumber { get; set; }

        public UserAddressDTO? Address { get; set; }
        public bool IsDefault { get; set; } = true;

        // Restaurant details (common)
        public string RestaurantName { get; set; } = string.Empty;
        public string CuisineType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int AveragePreparationTime { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }
}
