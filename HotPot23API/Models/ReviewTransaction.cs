using System.ComponentModel.DataAnnotations;

namespace HotPot23API.Models
{
    public class ReviewTransaction
    {
        public int ReviewID { get; set; }  // Primary key via Fluent API

        [Required(ErrorMessage = "UserID is required")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "MenuItemID is required")]
        public int MenuItemID { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot be longer than 500 characters")]
        public string Comment { get; set; } = string.Empty;

        [Required(ErrorMessage = "CreatedOn date is required")]
        public DateTime CreatedOn { get; set; }

        // Navigation properties
        public UserMaster? User { get; set; }
        public MenuItemMaster? MenuItem { get; set; }
    }
}
