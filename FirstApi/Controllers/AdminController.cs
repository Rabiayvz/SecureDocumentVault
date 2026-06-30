using FirstApi.Business.Services;
using FirstApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers()
        {
            var users = _adminService.GetAllUsers();
            return Ok(users);
        }

        [HttpPut("users/{id}/role")]
        [Authorize(Roles = "Admin")]
        public IActionResult AssignRole(Guid id, AssignRoleDto dto)
        {
            var success = _adminService.AssignRole(id, dto.RoleId);
            if (!success) return NotFound("User or role not found.");
            return Ok("Role assigned successfully.");
        }

        // GET /admin/users/my-team  → Manager: kendi takımı
        [HttpGet("users/my-team")]
        [Authorize(Roles = "Manager")]
        public IActionResult GetMyTeam()
        {
            var users = _adminService.GetMyTeam();
            return Ok(users);
        }

        // GET /admin/users/my-manager  → User: kendi manager'ını görür
        [HttpGet("users/my-manager")]
        [Authorize(Roles = "User")]
        public IActionResult GetMyManager()
        {
            var manager = _adminService.GetMyManager();
            if (manager == null) return NotFound("No manager assigned.");
            return Ok(manager);
        }
    }
}