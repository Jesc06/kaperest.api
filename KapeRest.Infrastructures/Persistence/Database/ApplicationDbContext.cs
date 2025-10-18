using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Domain.Entities.PendingAccounts;

namespace KapeRest.Infrastructures.Persistence.Database
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<PendingUserAccount> PendingUserAccount { get; set; }

    }
}
