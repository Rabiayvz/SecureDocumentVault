using FirstApi.Data;
using FirstApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace FirstApi.Business.Services
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditLogService _auditLogService;


        public AdminService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IAuditLogService auditLogService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _auditLogService = auditLogService;
        }

        public List<UserResponseDto> GetAllUsers()
        {
            return _context.Users
                .Include(u => u.Role)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role!.Name,
                    CreatedAt = u.CreatedAt
                }).ToList();
        }

        public bool AssignRole(Guid userId, int roleId)
        {
            var user = _context.Users.Find(userId);
            if (user == null) return false;

            var roleExists = _context.Roles.Any(r => r.Id == roleId);
            if (!roleExists) return false;

            user.RoleId = roleId;
            _context.SaveChanges();
            _auditLogService.Log("RoleAssigned", $"User {userId} assigned role {roleId}");
            return true;
        }

        public List<UserResponseDto> GetMyTeam()
        {
            var managerIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            var managerId = Guid.Parse(managerIdClaim!.Value);

            return _context.Users
                .Include(u => u.Role)
                .Where(u => u.ManagerId == managerId)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role!.Name,
                    CreatedAt = u.CreatedAt
                }).ToList();
        }

        public UserResponseDto? GetMyManager()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            var userId = Guid.Parse(userIdClaim!.Value);

            var user = _context.Users
                .Include(u => u.Manager)
                    .ThenInclude(m => m!.Role)
                .FirstOrDefault(u => u.Id == userId);

            if (user?.Manager == null) return null;

            return new UserResponseDto
            {
                Id = user.Manager.Id,
                Email = user.Manager.Email,
                Role = user.Manager.Role!.Name,
                CreatedAt = user.Manager.CreatedAt
            };
        }
    }
}