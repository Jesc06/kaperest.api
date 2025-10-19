using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Domain.Entities.PendingAccounts;
using KapeRest.Domain.Entities.Inventory;

namespace KapeRest.Infrastructures.Persistence.Database
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<PendingUserAccount> PendingUserAccount { get; set; }

        #region--Inventory DbSets --
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        #endregion

    }
}
