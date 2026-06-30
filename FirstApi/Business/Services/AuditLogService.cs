using FirstApi.Data;
using FirstApi.Models;
using System.Security.Claims;
using FirstApi.Dtos;

namespace FirstApi.Business.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private Guid? GetCurrentUserId()
        {
            var claim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null) return null;
            return Guid.Parse(claim.Value);
        }

        private string? GetCurrentIpAddress()
        {
            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }

        public void Log(string action, string details, Guid? userId = null)
        {
            var resolvedUserId = userId ?? GetCurrentUserId();
            if (resolvedUserId == null) return;

            var log = new AuditLog
            {
                UserId = resolvedUserId.Value,
                Action = action,
                Details = details,
                IpAddress = GetCurrentIpAddress(),
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            _context.SaveChanges();
        }

        public List<AuditLogResponseDto> GetAll()
        {
            return _context.AuditLogs
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new AuditLogResponseDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    Action = l.Action,
                    Details = l.Details,
                    IpAddress = l.IpAddress,
                    CreatedAt = l.CreatedAt
                }).ToList();
        }

        public List<AuditLogResponseDto> GetByUser(Guid userId)
        {
            return _context.AuditLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new AuditLogResponseDto
                {
                    Id = l.Id,
                    UserId = l.UserId,
                    Action = l.Action,
                    Details = l.Details,
                    IpAddress = l.IpAddress,
                    CreatedAt = l.CreatedAt
                }).ToList();
        }
        
    }
}