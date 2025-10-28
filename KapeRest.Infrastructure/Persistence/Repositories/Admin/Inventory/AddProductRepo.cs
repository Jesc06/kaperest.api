using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.SupplierEntities;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructures.Persistence.Repositories.Admin.Inventory
{
    public class AddProductRepo : IInventory
    {
        private readonly ApplicationDbContext _context;
        public AddProductRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> AddProductOfSuppliers(string currentUser,string role,CreateProductDTO addProduct)
        {
          var supplier = await _context.Suppliers
               .Include(s => s.TransactionHistories)
               .FirstOrDefaultAsync(s => s.Id == addProduct.SupplierId);

            if (supplier is null)   
                throw new Exception("Supplier does not exist.");

          var add = new ProductOfSupplier
          {
            ProductName = addProduct.ProductName,
            CostPrice = addProduct.CostPrice,
            Stocks = addProduct.Stocks,
            Units = addProduct.Units,
            SupplierId = addProduct.SupplierId,
          };
          
          await _context.Products.AddAsync(add);
            
          supplier.TransactionHistories.Add(new SupplierTransactionHistory
          {
             User = currentUser,
             Action = "Added",
             SupplierId = supplier.Id,
             ProductName = addProduct.ProductName,
             Price = addProduct.CostPrice.ToString("C"),
             QuantityDelivered = addProduct.Stocks,
             TotalCost = addProduct.CostPrice * addProduct.Stocks,
             TransactionDate = DateTime.Now
          });

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = currentUser,
                Role = role,
                Category = "Inventory",
                Action = "Added",
                AffectedEntity = addProduct.ProductName,
                Description = $"Added product {addProduct.ProductName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
          
          return "Successfully added products";
        }

        public async Task<string> UpdateProductOfSuppliers(string currentUser,string role,UpdateProductDTO update)
        {
          
            var product = await _context.Products.FindAsync(update.Id);
            if (product == null)
                throw new Exception("Product not found.");
  
            product.ProductName = update.ProductName ?? product.ProductName;
            product.CostPrice = update.Prices ?? product.CostPrice;
            product.Stocks = update.Stocks ?? product.Stocks;
            product.Units = update.Units ?? product.Units;

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = currentUser,
                Role = role,
                Category = "Inventory", 
                Action = "Updated",
                AffectedEntity = product.ProductName,
                Description = $"Updated product {product.ProductName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            var supplier = await _context.Suppliers.FindAsync(product.SupplierId);

            return "Successfully updated products";
        }

        public async Task<bool> DeleteProductOfSuppliers(string currentUser,string role, int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Product not found.");

            _context.Products.Remove(product);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = currentUser,
                Role  = role,
                Category = "Inventory",
                Action = "Deleted",
                AffectedEntity = product.ProductName,
                Description = $"Deleted product {product.ProductName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ICollection> GetAllProducts()
        {
            var products = await _context.Products
                 .Select(p => new
                 {
                    p.ProductName,
                    p.Stocks,
                    p.Units,
                    p.CostPrice,
                    p.TransactionDate,
                    p.Supplier.SupplierName,
                 }).ToListAsync();
            return products;
        }




    }
}
