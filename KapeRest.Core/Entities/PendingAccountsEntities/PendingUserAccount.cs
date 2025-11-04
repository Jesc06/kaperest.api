using KapeRest.Core.Entities.Branch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Domain.Entities.PendingAccounts
{
    public class PendingUserAccount
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = "User";
        public string Status { get; set; } = "Pending"; 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? BranchId { get; set; }
        public BranchEntities? Branch { get; set; }

        //Staff to Cashier link
        public string? CashierId { get; set; }
    }
}
