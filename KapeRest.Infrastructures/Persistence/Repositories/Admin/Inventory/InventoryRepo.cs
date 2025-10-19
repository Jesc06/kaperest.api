using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Domain.Entities.Inventory;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace KapeRest.Infrastructures.Persistence.Repositories.Admin.Inventory
{
    public class InventoryRepo : IInventory
    {
        private readonly ApplicationDbContext _context;
        public InventoryRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductResponseDTO> AddProduct(CreateProductDTO addProduct)
        {
          var supplier = await _context.Suppliers
               .Include(s => s.TransactionHistories)
               .FirstOrDefaultAsync(s => s.Id == addProduct.SupplierId);
            if (supplier is null)
                throw new Exception("Supplier does not exist.");

          var add = new Product
          {
            Name = addProduct.Name,
            Category = addProduct.Category,
            Price = addProduct.Price,
            Quantity = addProduct.Quantity,
            ReorderLevel = addProduct.ReorderLevel,
            SupplierId = addProduct.SupplierId
          };
          
          await _context.Products.AddAsync(add);

          supplier.TransactionHistories.Add(new SupplierTransactionHistory
          {
             SupplierId = supplier.Id,
             ProductName = addProduct.Name,
             QuantityDelivered = addProduct.Quantity,
             TotalCost = addProduct.Price * addProduct.Quantity,
             TransactionDate = DateTime.Now
          });

          await _context.SaveChangesAsync();
            
          var response = new ProductResponseDTO
          {
            Id = add.Id,
            Name = add.Name,
            Category = add.Category,
            Price = add.Price,
            Quantity = add.Quantity,
            SupplierName = supplier.Name
          };
          
          return response;
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
