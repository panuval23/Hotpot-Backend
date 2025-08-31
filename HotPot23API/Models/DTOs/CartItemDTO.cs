namespace HotPot23API.Models.DTOs
{
    public class CartItemDTO
    {
        public int CartID { get; set; }
        public int MenuItemID { get; set; }

        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string RestaurantAddress { get; set; } = string.Empty;
        public string RestaurantImage { get; set; } = string.Empty;

        public string MenuItemName { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal FinalPrice => DiscountPercent.HasValue
            ? OriginalPrice * (1 - (DiscountPercent.Value / 100))
            : OriginalPrice;
        public int Quantity { get; set; }
        public decimal Subtotal => FinalPrice * Quantity;
    }
}
