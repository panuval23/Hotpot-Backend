namespace HotPot23API.Models.DTOs
{
    public class RestaurantUpdateDTO
    {
        public int RestaurantID { get; set; }   // existing restaurant
        public string RestaurantName { get; set; } = string.Empty;
        public string CuisineType { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int AveragePreparationTime { get; set; }
        public bool IsAvailable { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
    }
}
