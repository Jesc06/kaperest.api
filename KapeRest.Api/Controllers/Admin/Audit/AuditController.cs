using KapeRest.Application.Services.Admin.Audit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Admin.Audit
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly AuditService _auditService;
        public AuditController(AuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("GetALlAudit")]
        public async Task<ActionResult> GetALlAudit()
        {
            var result = await _auditService.GetAudit();
            return Ok(result);
        }   

    }
}
