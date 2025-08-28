namespace HotPot23API.Models.DTOs
{
    public class UserRegisterResponseDTO
    {
        public int UserID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Message { get; set; } = "Registered Successfully";
    }
}
