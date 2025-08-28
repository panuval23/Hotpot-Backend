namespace HotPot23API.Models.DTOs
{
    public class OrderResponseDTO
    {
        public int OrderID { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemDTO> Items { get; set; }

        public int RestaurantID { get; set; }
        public string RestaurantName { get; set; } = string.Empty;

        public List<DeliveryStatusDTO>? DeliveryStatuses { get; set; }
    }
}
