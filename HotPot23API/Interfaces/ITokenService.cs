using HotPot23API.Models.DTOs;

namespace HotPot23API.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateToken(TokenUser user);
    }
}
