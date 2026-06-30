using FirstApi.Business.Services;
using FirstApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDto dto)
        {
            var userId = _authService.Register(dto);
            return Ok(userId);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var token = _authService.Login(dto);
            if (token == null) return Unauthorized("Email veya şifre hatalı.");

            return Ok(new AuthResponseDto
            {
                Token = token,
                Email = dto.Email
            });
        }
    }
}