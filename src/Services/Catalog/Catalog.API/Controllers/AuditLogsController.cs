using Microsoft.AspNetCore.Mvc;
using Catalog.API.Data.Interface;
using Catalog.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/audit-logs")]
    public class AuditLogsController : ControllerBase
    {
        private readonly IAuditLogRepository _auditLogRepository;

        public AuditLogsController(IAuditLogRepository auditLogRepository)
        {
            _auditLogRepository = auditLogRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs()
        {
            var logs = await _auditLogRepository.GetAllAsync();
            return Ok(logs);
        }
    }
}