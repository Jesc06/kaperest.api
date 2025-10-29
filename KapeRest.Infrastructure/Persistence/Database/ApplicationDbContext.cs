using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Domain.Entities.PendingAccounts;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.SupplierEntities;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.MenuEntities;
using KapeRest.Core.Entities.Tax_Rate;
using KapeRest.Core.Entities.Branch;

namespace KapeRest.Infrastructures.Persistence.Database
{
    public class ApplicationDbContext : IdentityDbContext<UsersIdentity>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<PendingUserAccount> PendingUserAccount { get; set; }
        public DbSet<Tax> Tax { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemProduct> MenuItemProducts { get; set; }
        public DbSet<BranchEntities> Branches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MenuItemProduct>()
                .HasKey(mip => new { mip.MenuItemId, mip.ProductOfSupplierId });

            modelBuilder.Entity<MenuItemProduct>()
                .HasOne(mip => mip.MenuItem)
                .WithMany(mi => mi.MenuItemProducts)
                .HasForeignKey(mip => mip.MenuItemId);

            modelBuilder.Entity<MenuItemProduct>()
                .HasOne(mip => mip.ProductOfSupplier)
                .WithMany()
                .HasForeignKey(mip => mip.ProductOfSupplierId);

            //for branch relationship
            modelBuilder.Entity<PendingUserAccount>()
            .HasOne(p => p.Branch)
            .WithMany(b => b.PendingAccounts)
            .HasForeignKey(p => p.BranchId)
            .OnDelete(DeleteBehavior.Restrict);
        }

        #region--Inventory DbSets --
        public DbSet<AddSupplier> Suppliers { get; set; }
        public DbSet<SupplierTransactionHistory> SupplierTransactionHistories { get; set; }
        public DbSet<AuditLogEntities> AuditLog { get; set; }
        public DbSet<ProductOfSupplier> Products { get; set; }
        #endregion

    }
}
