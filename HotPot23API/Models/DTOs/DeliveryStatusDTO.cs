namespace HotPot23API.Models.DTOs
{
    public class DeliveryStatusDTO
    {
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public int StatusUpdatedBy { get; set; }
    }
}
