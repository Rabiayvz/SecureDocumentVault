using FirstApi.Dtos;

namespace FirstApi.Business.Services
{
    public interface IAuthService
    {
        Guid Register(RegisterDto dto);
        string? Login(LoginDto dto);
    }
}