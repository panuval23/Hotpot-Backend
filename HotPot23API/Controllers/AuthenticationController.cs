using HotPot23API.Interfaces;
using HotPot23API.Models.DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace HotPot23API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("DefaultCORS")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticate _authService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(IAuthenticate authService, ILogger<AuthenticationController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserRegisterResponseDTO>> Register(UserRegisterDTO dto)
        {
            try
            {
                var result = await _authService.Register(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO dto)
        {
            try
            {
                var token = await _authService.Login(dto);
                return Ok(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unauthorized access done by username " + dto.Username);
                return Unauthorized(new ErrorObjectDTO { ErrorNumber = 401, ErrorMessage = "Invalid username or password" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO dto)
        {
            try
            {
                await _authService.ForgotPassword(dto.Email);
                return Ok(new { Message = "Login credentials sent to your email." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Forgot password failed for {Email}", dto.Email);
                return BadRequest(new { Message = "Unable to process forgot password request." });
            }
        }

    }
}
