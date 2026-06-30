using FirstApi.Dtos;

namespace FirstApi.Business.Services
{
    public interface IAuditLogService
    {
        void Log(string action, string details, Guid? userId = null);
        List<AuditLogResponseDto> GetAll();
        List<AuditLogResponseDto> GetByUser(Guid userId);
    }
}