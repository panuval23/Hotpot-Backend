namespace HotPot23API.Models.DTOs
{
    public class PlaceOrderRequestDTO
    {
        public List<CartItemDTO> Items { get; set; } = new();
        public int DeliveryAddressID { get; set; }
        public string PaymentMethod { get; set; } = "COD"; // Default Cash on Delivery

    }
}
