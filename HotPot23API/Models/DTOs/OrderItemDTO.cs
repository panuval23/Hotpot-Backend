namespace HotPot23API.Models.DTOs
{
    public class OrderItemDTO
    {
        public int MenuItemID {  get; set; }
        public string MenuItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => Price * Quantity;
    }
}
