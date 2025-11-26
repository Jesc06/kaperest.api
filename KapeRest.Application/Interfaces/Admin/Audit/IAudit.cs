using KapeRest.Domain.Entities.AuditLogEntities;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Admin.Audit
{
    public interface IAudit
    {

        Task<List<AuditLogEntities>> GetAuditTrail();

    }
}
