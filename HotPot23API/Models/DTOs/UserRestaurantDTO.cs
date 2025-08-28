namespace HotPot23API.Models.DTOs
{
    public class UserRestaurantDTO
    {
        public int RestaurantID { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string CuisineType { get; set; }
        public string? ImageUrl { get; set; }


    }
}
