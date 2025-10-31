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
using KapeRest.Core.Entities.SalesTransaction;

namespace KapeRest.Infrastructures.Persistence.Database
{
    public class ApplicationDbContext : IdentityDbContext<UsersIdentity>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Many-to-many for MenuItemProduct
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

            // Branch relationship
            modelBuilder.Entity<PendingUserAccount>()
                .HasOne(p => p.Branch)
                .WithMany(b => b.PendingAccounts)
                .HasForeignKey(p => p.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship for sales, branch, and cashier
            modelBuilder.Entity<SalesTransactionEntities>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasOne<UsersIdentity>()
                    .WithMany()
                    .HasForeignKey(x => x.CashierId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<BranchEntities>()
                  .WithMany()
                  .HasForeignKey(x => x.BranchId)
                  .OnDelete(DeleteBehavior.Restrict);

            });
        }


        #region--Accounts--
        public DbSet<PendingUserAccount> PendingUserAccount { get; set; }
        public DbSet<UsersIdentity> UsersIdentity { get; set; }
        #endregion

        #region--Inventory DbSets --
        public DbSet<AddSupplier> Suppliers { get; set; }
        public DbSet<SupplierTransactionHistory> SupplierTransactionHistories { get; set; }
        public DbSet<ProductOfSupplier> Products { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemProduct> MenuItemProducts { get; set; }
        #endregion

        #region--Sales Transaction--
        public DbSet<SalesTransactionEntities> SalesTransaction { get; set; }
        #endregion

        #region--Auditlogs--
        public DbSet<AuditLogEntities> AuditLog { get; set; }
        #endregion

        #region--Branches,Tax,Discounts--
        public DbSet<Tax> Tax { get; set; }
        public DbSet<Discount> Discount { get; set; }
        public DbSet<BranchEntities> Branches { get; set; }
        #endregion

    }
}
