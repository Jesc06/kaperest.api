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
        public async Task<ProductResponseDTO> AddProduct(string currentUser,string role,CreateProductDTO addProduct)
        {
          var supplier = await _context.Suppliers
               .Include(s => s.TransactionHistories)
               .FirstOrDefaultAsync(s => s.Id == addProduct.SupplierId);

            if (supplier is null)
                throw new Exception("Supplier does not exist.");

            // Convert Base64 to byte array
            byte[]? imageBytes = string.IsNullOrEmpty(addProduct.Base64Image)
                ? null
                : Convert.FromBase64String(addProduct.Base64Image);

          var add = new AddProduct
          {
            ProductName = addProduct.ProductName,
            Category = addProduct.Category,
            Price = addProduct.Price,
            Quantity = addProduct.Quantity,
            ReorderLevel = addProduct.ReorderLevel,
            SupplierId = addProduct.SupplierId,
            ImageBase64 = imageBytes,
            ImageMimeType = addProduct.ImageMimeType
          };
          
          await _context.Products.AddAsync(add);
            
          supplier.TransactionHistories.Add(new SupplierTransactionHistory
          {
             User = currentUser,
             Action = "Added",
             SupplierId = supplier.Id,
             ProductName = addProduct.ProductName,
             Price = addProduct.Price.ToString("C"),
             QuantityDelivered = addProduct.Quantity,
             TotalCost = addProduct.Price * addProduct.Quantity,
             TransactionDate = DateTime.Now
          });

            _context.AuditLog.Add(new AuditLogEntities
            {
                User = currentUser,
                Role = role,
                Module = "Inventory",
                Action = "Added",
                Target = addProduct.ProductName,
                Description = $"Added product {addProduct.ProductName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

          // Convert image back to Base64 for response
          string responseBase64 = add.ImageBase64 != null ? Convert.ToBase64String(add.ImageBase64) : null;
       
          var response = new ProductResponseDTO
          {
            Id = add.Id,
            ProductName = add.ProductName,
            Category = add.Category,
            Price = add.Price,
            Quantity = add.Quantity,
            SupplierName = supplier.SupplierName,
            Base64Image = responseBase64,
            ImageMimeType = add.ImageMimeType
          };
          
          return response;
        }

        public async Task<ProductResponseDTO> UpdateProduct(string currentUser,string role,UpdateProductDTO update)
        {
          
            var product = await _context.Products.FindAsync(update.Id);
            if (product == null)
                throw new Exception("Product not found.");
  
            // Update basic info
            product.ProductName = update.ProductName ?? product.ProductName;
            product.Category = update.Category ?? product.Category;
            product.Price = update.Price ?? product.Price;
            product.Quantity = update.Quantity ?? product.Quantity;
            product.ReorderLevel = update.ReorderLevel ?? product.ReorderLevel;

            // Update image if new one provided
            if (!string.IsNullOrEmpty(update.Base64Image))
            {
                product.ImageBase64 = Convert.FromBase64String(update.Base64Image);
                product.ImageMimeType = update.ImageMimeType;
            }

            _context.AuditLog.Add(new AuditLogEntities
            {
                User = currentUser,
                Role = role,
                Module = "Inventory",
                Action = "Updated",
                Target = product.ProductName,
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
                Quantity = product.Quantity,
                SupplierName = supplier?.SupplierName,
                Base64Image = product.ImageBase64 != null ? Convert.ToBase64String(product.ImageBase64) : null,
                ImageMimeType = product.ImageMimeType
            };

            return response;
        }

        public async Task<bool> DeleteProduct(string currentUser,string role, int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
                throw new Exception("Product not found.");

            _context.Products.Remove(product);

            _context.AuditLog.Add(new AuditLogEntities
            {
                User = currentUser,
                Role  = role,
                Module = "Inventory",
                Action = "Deleted",
                Target = product.ProductName,
                Description = $"Deleted product {product.ProductName}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return true;
        }




    }
}
