using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.AuditLogEntities
{
    public class AuditLogEntities
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Role { get; set; }
        public string Module { get; set; }//Supplier,Product,Login
        public string Action { get; set; }//Add,Delete,Deliver
        public string Target { get; set; }//SupplierName or ProductName
        public string? Description { get; set; }//Delivered 50 units of Latte
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
