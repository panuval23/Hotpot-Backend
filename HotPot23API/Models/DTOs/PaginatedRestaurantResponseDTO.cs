namespace HotPot23API.Models.DTOs
{
    public class PaginatedRestaurantResponseDTO
    {
        public List<RestaurantResponseDTO> Restaurants { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public int PageNumber { get; set; }
    }
}
