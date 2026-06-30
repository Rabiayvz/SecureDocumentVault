using FirstApi.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstApi.Controllers
{
    [ApiController]
    [Route("audit-logs")]
    [Authorize(Roles = "Admin,Auditor")]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var logs = _auditLogService.GetAll();
            return Ok(logs);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetByUser(Guid userId)
        {
            var logs = _auditLogService.GetByUser(userId);
            return Ok(logs);
        }
    }
}