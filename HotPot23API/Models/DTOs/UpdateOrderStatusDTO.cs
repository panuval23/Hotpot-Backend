using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models.DTOs
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        public string NewStatus { get; set; } = string.Empty;
    }
}
