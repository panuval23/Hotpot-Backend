namespace HotPot23API.Models.DTOs
{
    public class UserMenuItemResponseDTO
    {
        public int MenuItemID { get; set; }
        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; }
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string AvailabilityTime { get; set; } = string.Empty;
        public bool IsVeg { get; set; }
        public string TasteInfo { get; set; } = string.Empty;
        public string NutritionalInfo { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public bool InStock { get; set; }

        // Discount fields
        public decimal? DiscountPercent { get; set; } // null if no discount
        public decimal FinalPrice { get; set; }       // Price after discount, or same as Price if no discount

        public DateTime? DiscountValidFrom { get; set; }
        public DateTime? DiscountValidTo { get; set; }
    }
}
