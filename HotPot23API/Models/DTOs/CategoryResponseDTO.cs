namespace HotPot23API.Models.DTOs
{
    public class CategoryResponseDTO
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
