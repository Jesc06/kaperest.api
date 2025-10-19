using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.SupplierEntities;
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

            // Convert Base64 to byte array
            byte[]? imageBytes = string.IsNullOrEmpty(addProduct.Base64Image)
                ? null
                : Convert.FromBase64String(addProduct.Base64Image);


           var add = new Product
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
             SupplierId = supplier.Id,
             ProductName = addProduct.ProductName,
             QuantityDelivered = addProduct.Quantity,
             TotalCost = addProduct.Price * addProduct.Quantity,
             TransactionDate = DateTime.Now
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

    }
}
