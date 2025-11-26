using KapeRest.Application.Interfaces.Admin.Audit;
using KapeRest.Domain.Entities.AuditLogEntities;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Admin.Audit
{
    public class AuditService
    {
        private readonly IAudit _audit;
        public AuditService(IAudit audit)
        {
            _audit = audit;
        }

        public async Task<List<AuditLogEntities>> GetAudit()
        {
            var result = await _audit.GetAuditTrail();
            return result;
        }


    }
}
