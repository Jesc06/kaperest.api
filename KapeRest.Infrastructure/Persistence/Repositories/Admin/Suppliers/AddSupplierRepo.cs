using KapeRest.Application.DTOs.Admin.Supplier;
using KapeRest.Application.Interfaces.Admin.Supplier;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.SupplierEntities;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructures.Persistence.Repositories.Admin.Suppliers
{
    public class AddSupplierRepo : ISupplier
    {
        private readonly ApplicationDbContext _context;
        public AddSupplierRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupplierResponseDTO> AddSupplier(CreateSupplierDTO addSupplier)
        {
            var supplier = new AddSupplier
            {
                SupplierName = addSupplier.SupplierName,
                ContactPerson = addSupplier.ContactPerson,
                PhoneNumber = addSupplier.PhoneNumber,
                Email = addSupplier.Email,
                Address = addSupplier.Address,
                Products = new List<ProductOfSupplier>(),
                TransactionHistories = new List<SupplierTransactionHistory>()
            };

            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();

            var response = new SupplierResponseDTO
            {
                Id = supplier.Id,
                SupplierName = supplier.SupplierName,
                ContactPerson = supplier.ContactPerson,
                PhoneNumber = supplier.PhoneNumber,
                Email = supplier.Email,
                Address = supplier.Address,
                Transactions = supplier.TransactionHistories?
                  .Select(t => $"{t.FormattedDate} - {t.ProductName} ({t.QuantityDelivered}) = {t.TotalCost:C}")
                  .ToList() ?? new List<string>()
            };

            return response;
        }


        public async Task<SupplierResponseDTO> UpdateSupplier(UpdateSupplierDTO update)
        {
            var product = await _context.Suppliers.FindAsync(update.Id);
            if (product == null)
                throw new Exception("Product not found.");

            product.SupplierName = update.SupplierName ?? product.SupplierName;
            product.ContactPerson = update.ContactPerson ?? product.ContactPerson;
            product.PhoneNumber = update.PhoneNumber ?? product.PhoneNumber;
            product.Email = update.Email ?? product.Email;
            product.Address = update.Address ?? product.Address;

            await _context.SaveChangesAsync();

            var response = new SupplierResponseDTO
            {
                Id = product.Id,
                SupplierName = product.SupplierName,
                ContactPerson = product.ContactPerson,
                PhoneNumber = product.PhoneNumber,
                Email = product.Email,
                Address = product.Address,
                Transactions = product.TransactionHistories?
                  .Select(t => $"{t.FormattedDate} - {t.ProductName} ({t.QuantityDelivered}) = {t.TotalCost:C}")
                  .ToList() ?? new List<string>()
            };

            return response;

        }


        public async Task<bool> DeleteSupplier(int productId)
        {
            var product = await _context.Suppliers.FindAsync(productId);
            if (product == null)
                throw new Exception("Product not found.");

            _context.Suppliers.Remove(product);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ICollection> GetAllSupplier()
        {
            var suppliers = await _context.Suppliers
                .Select(s => new
                {
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

                }).ToListAsync();
            return suppliers;
        }


    }
}
