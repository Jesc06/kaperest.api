using KapeRest.Core.Entities.Branch;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructures.Persistence.Database
{
    public class UsersIdentity : IdentityUser
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }  
        public string LastName { get; set; }

        public int? BranchId { get; set; }
        public BranchEntities? Branch { get; set; }

        //Link Staff → Cashier
        public string? CashierId { get; set; }
        public UsersIdentity? Cashier { get; set; }


        //Jwt refresh token properties
        public string? RefreshTokenHash { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

    }
}
