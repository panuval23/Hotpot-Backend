namespace HotPot23API.Models.DTOs
{
    public class MenuItemUpdateDTO
    {
        public int CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string AvailabilityTime { get; set; } = string.Empty;
        public bool IsVeg { get; set; }
        public string TasteInfo { get; set; } = string.Empty;
        public string NutritionalInfo { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool InStock { get; set; }
    }
}
