using KapeRest.Application.Interfaces.Admin.Audit;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Infrastructure.Persistence.Database;

namespace KapeRest.Infrastructure.Persistence.Repositories.Admin.Audit
{
    public class AuditRepository : IAudit
    {
        private readonly ApplicationDbContext _context;
        public AuditRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<AuditLogEntities>> GetAuditTrail()
        {
            var audit = await _context.AuditLog.ToListAsync();
            return audit;
        }
        
    }
}
