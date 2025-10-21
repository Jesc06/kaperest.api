using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.SupplierEntities;
using KapeRest.Infrastructures.Migrations;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task<ProductResponseDTO> AddProductOfSuppliers(string currentUser,string role,CreateProductDTO addProduct)
        {
          var supplier = await _context.Suppliers
               .Include(s => s.TransactionHistories)
               .FirstOrDefaultAsync(s => s.Id == addProduct.SupplierId);

            if (supplier is null)
                throw new Exception("Supplier does not exist.");

          var add = new ProductOfSupplier
          {
            ProductName = addProduct.ProductName,
            Category = addProduct.Category,
            Price = addProduct.Price,
            Stock = addProduct.Stock,
            SupplierId = addProduct.SupplierId,
          };
          
          await _context.Products.AddAsync(add);
            
          supplier.TransactionHistories.Add(new SupplierTransactionHistory
          {
             User = currentUser,
             Action = "Added",
             SupplierId = supplier.Id,
             ProductName = addProduct.ProductName,
             Price = addProduct.Price.ToString("C"),
             QuantityDelivered = addProduct.Stock,
             TotalCost = addProduct.Price * addProduct.Stock,
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
       
          var response = new ProductResponseDTO
          {
            Id = add.Id,
            ProductName = add.ProductName,
            Category = add.Category,
            Price = add.Price,
            Stock = add.Stock,
            SupplierName = supplier.SupplierName,
          };
          
          return response;
        }

        public async Task<ProductResponseDTO> UpdateProductOfSuppliers(string currentUser,string role,UpdateProductDTO update)
        {
          
            var product = await _context.Products.FindAsync(update.Id);
            if (product == null)
                throw new Exception("Product not found.");
  
            // Update basic info
            product.ProductName = update.ProductName ?? product.ProductName;
            product.Category = update.Category ?? product.Category;
            product.Price = update.Price ?? product.Price;
            product.Stock = update.Stock ?? product.Stock;

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

            var response = new ProductResponseDTO
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Category = product.Category,
                Price = product.Price,
                Stock = product.Stock,
                SupplierName = supplier?.SupplierName,
            };

            return response;
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




    }
}
