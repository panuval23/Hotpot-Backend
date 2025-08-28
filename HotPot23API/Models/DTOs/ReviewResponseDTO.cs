namespace HotPot23API.Models.DTOs
{
    public class ReviewResponseDTO
    {
        public int ReviewID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int MenuItemID { get; set; }
        public string MenuItemName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
    }
}
