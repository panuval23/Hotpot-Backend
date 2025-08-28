namespace HotPot23API.Models.DTOs
{
    public class TokenUser
    {
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? RestaurantID { get; set; } // nullable int for restaurant id

        public int? UserID {  get; set; }
    }
}
