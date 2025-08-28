namespace HotPot23API.Models.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string ContactNumber { get; set; }
        public List<UserAddressDTO> Addresses { get; set; } = new();
    }
}
