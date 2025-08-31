namespace HotPot23API.Models.DTOs
{
    public class RestaurantResponseDTO
    {
        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Message { get; set; } = "Restaurant registered successfully";
    }
}
