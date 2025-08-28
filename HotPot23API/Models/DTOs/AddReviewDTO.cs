namespace HotPot23API.Models.DTOs
{
    public class AddReviewDTO
    {
        public int MenuItemID { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
