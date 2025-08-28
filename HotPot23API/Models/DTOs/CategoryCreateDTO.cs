namespace HotPot23API.Models.DTOs
{
    public class CategoryCreateDTO
    {
        public string CategoryName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
