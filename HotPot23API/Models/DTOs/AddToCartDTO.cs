namespace HotPot23API.Models.DTOs
{
    public class AddToCartDTO
    {
        public int RestaurantID { get; set; }
        public int MenuItemID { get; set; }
        public int Quantity { get; set; }
    }
}
