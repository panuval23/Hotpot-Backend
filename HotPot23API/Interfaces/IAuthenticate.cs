using HotPot23API.Models.DTOs;

namespace HotPot23API.Interfaces
{
    public interface IAuthenticate
    {
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequest);
        Task<UserRegisterResponseDTO> Register(UserRegisterDTO userRegister);

        Task ForgotPassword(string email);
    }
}
