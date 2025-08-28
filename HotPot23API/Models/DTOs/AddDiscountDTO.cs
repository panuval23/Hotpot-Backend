namespace HotPot23API.Models.DTOs
{
    public class AddDiscountDTO
    {
        public int MenuItemID { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
