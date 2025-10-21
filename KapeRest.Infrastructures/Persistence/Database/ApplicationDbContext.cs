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

namespace KapeRest.Infrastructures.Persistence.Database
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<PendingUserAccount> PendingUserAccount { get; set; }

        #region--Inventory DbSets --

        public DbSet<AddSupplier> Suppliers { get; set; }
        public DbSet<SupplierTransactionHistory> SupplierTransactionHistories { get; set; }
        public DbSet<AuditLogEntities> AuditLog { get; set; }

        public DbSet<ProductOfSupplier> Products { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuItemProduct> MenuItemProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MenuItemProduct>()
                .HasKey(mp => new { mp.MenuItemId, mp.ProductId });

            modelBuilder.Entity<MenuItemProduct>()
                .HasOne(mp => mp.MenuItem)
                .WithMany(m => m.MenuItemProducts)
                .HasForeignKey(mp => mp.MenuItemId);

            modelBuilder.Entity<MenuItemProduct>()
                .HasOne(mp => mp.Product)
                .WithMany()
                .HasForeignKey(mp => mp.ProductId);
        }
        #endregion

    }
}
