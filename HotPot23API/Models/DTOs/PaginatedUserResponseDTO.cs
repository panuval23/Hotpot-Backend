namespace HotPot23API.Models.DTOs
{
    public class PaginatedUserResponseDTO
    {
        public List<UserDTO> Users { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public int PageNumber { get; set; }
    }
}
