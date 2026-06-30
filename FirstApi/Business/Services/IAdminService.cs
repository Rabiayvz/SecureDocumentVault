using FirstApi.Dtos;

namespace FirstApi.Business.Services
{
    public interface IAdminService
    {
        List<UserResponseDto> GetAllUsers();
        bool AssignRole(Guid userId, int roleId);
        List<UserResponseDto> GetMyTeam();
        UserResponseDto? GetMyManager();
    }
}