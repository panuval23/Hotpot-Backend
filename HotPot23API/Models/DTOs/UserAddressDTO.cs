namespace HotPot23API.Models.DTOs
{
    public class UserAddressDTO
    {
        public int AddressID { get; set; }
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public string Landmark { get; set; }
        public string AddressType { get; set; }
    }
}
