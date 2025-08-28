namespace HotPot23API.Models.DTOs
{
    public class CartResponseDTO
    {
        public List<CartItemDTO> Items { get; set; } = new();
        public decimal TotalCost => Items.Sum(i => i.Subtotal);
    }
}
