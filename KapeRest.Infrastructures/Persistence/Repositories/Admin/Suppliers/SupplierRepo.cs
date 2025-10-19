using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Supplier;
using KapeRest.Domain.Entities.Inventory;
using KapeRest.Infrastructures.Persistence.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructures.Persistence.Repositories.Admin.Suppliers
{
    public class SupplierRepo : ISupplier
    {
        private readonly ApplicationDbContext _context;
        public SupplierRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupplierResponseDTO> AddSupplier(CreateSupplierDTO addSupplier)
        {
            var supplier = new Supplier
            {
                Name = addSupplier.Name,
                Contact = addSupplier.Contact,
                Address = addSupplier.Address,
                Products = new List<Product>(),
                TransactionHistories = new List<SupplierTransactionHistory>()
            };

            await _context.Suppliers.AddAsync(supplier);
            await _context.SaveChangesAsync();

            var response = new SupplierResponseDTO
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Contact = supplier.Contact,
                Address = supplier.Address,
                Transactions = supplier.TransactionHistories?
                  .Select(t => $"{t.FormattedDate} - {t.ProductName} ({t.QuantityDelivered}) = {t.TotalCost:C}")
                  .ToList() ?? new List<string>()
            };

            return response;
        }



    }
}
