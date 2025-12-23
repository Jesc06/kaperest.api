using KapeRest.Application.DTOs.Admin.Supplier;
using KapeRest.Application.Interfaces.Admin.Supplier;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.SupplierEntities;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KapeRest.Infrastructure.Persistence.Database;

namespace KapeRest.Infrastructure.Persistence.Repositories.Admin.Suppliers
{
    public class AddSupplierRepository : ISupplier
    {
        private readonly ApplicationDbContext _context;
        public AddSupplierRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<SupplierResponseDTO> AddSupplier(CreateSupplierDTO addSupplier,string user, string role)
        {
            var supplier = new AddSupplier
            {
                SupplierName = addSupplier.SupplierName,
                ContactPerson = addSupplier.ContactPerson,
                PhoneNumber = addSupplier.PhoneNumber,
                Email = addSupplier.Email,
                Address = addSupplier.Address,
                UserId = addSupplier.UserId, // ADD THIS
                Products = new List<ProductOfSupplier>(),
                TransactionHistories = new List<SupplierTransactionHistory>()
            };

            await _context.Suppliers.AddAsync(supplier);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = user,
                Role = role,
                Action = "Added",
                Description = $"Added supplier {addSupplier.SupplierName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return new SupplierResponseDTO
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                ContactPerson = supplier.ContactPerson,
                PhoneNumber = supplier.PhoneNumber,
                Email = supplier.Email,
                Address = supplier.Address,
                Transactions = supplier.TransactionHistories?.Select(t =>
                    $"{t.FormattedDate} - {t.ProductName} ({t.QuantityDelivered}) = {t.TotalCost:C}"
                ).ToList()
            };
        }

        public async Task<SupplierResponseDTO> UpdateSupplier(UpdateSupplierDTO update, string userId)
        {
            var product = await _context.Suppliers
        .Where(s => s.Id == update.Id && s.UserId == userId)
        .FirstOrDefaultAsync();

            if (product == null)
                throw new Exception("Not found or unauthorized.");

            product.SupplierName = update.SupplierName ?? product.SupplierName;
            product.ContactPerson = update.ContactPerson ?? product.ContactPerson;
            product.PhoneNumber = update.PhoneNumber ?? product.PhoneNumber;
            product.Email = update.Email ?? product.Email;
            product.Address = update.Address ?? product.Address;

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = userId,
                Role = "Admin",
                Action = "Updated",
                Description = $"Updated supplier {product.SupplierName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return new SupplierResponseDTO
            {
                Id = product.Id,
                SupplierName = product.SupplierName,
                ContactPerson = product.ContactPerson,
                PhoneNumber = product.PhoneNumber,
                Email = product.Email,
                Address = product.Address,
                Transactions = product.TransactionHistories?.Select(t =>
                    $"{t.FormattedDate} - {t.ProductName} ({t.QuantityDelivered}) = {t.TotalCost:C}"
                ).ToList()
            };
        }

        public async Task<string> DeleteSupplier(int productId, string userId, string role)
        {
            var product = await _context.Suppliers.FindAsync(productId);
            if (product == null)
                return "Product not found.";

            _context.Suppliers.Remove(product);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = userId,
                Role = role,
                Action = "Deleted",
                Description = $"Deleted supplier {product.SupplierName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return "Successfully deleted";
        }

        public async Task<ICollection> GetAllSupplier(string userId)
        {
            var suppliers = await _context.Suppliers
         .Where(s => s.UserId == userId)   // FILTER BY USER
         .Select(s => new
         {
             s.Id,
             s.SupplierName,
             s.ContactPerson,
             s.PhoneNumber,
             s.Email,
             s.Address,
             s.TransactionDate,
             ProductOfSupplier = s.Products.Select(p => new
             {
                 p.ProductName,
                 p.Units,
             }),
         })
         .ToListAsync();
            return suppliers;
        }



    }
}
