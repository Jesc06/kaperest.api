using KapeRest.Application.DTOs.Admin.Inventory;
using KapeRest.Application.Interfaces.Admin.Inventory;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.InventoryEntities;
using KapeRest.Domain.Entities.SupplierEntities;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KapeRest.Infrastructure.Persistence.Database;

namespace KapeRest.Infrastructure.Persistence.Repositories.Admin.Inventory
{
    public class AddProductRepository : IInventory
    {
        private readonly ApplicationDbContext _context;

        public AddProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<string> AddProductOfSuppliers(string currentUser, string role, CreateProductDTO addProduct)
        {
            var supplier = await _context.Suppliers
                .Include(s => s.TransactionHistories)
                .FirstOrDefaultAsync(s => s.Id == addProduct.SupplierId);
            if (supplier is null)
                return "Supplier does not exist.";
            var product = new ProductOfSupplier
            {
                ProductName = addProduct.ProductName,
                CostPrice = addProduct.CostPrice,
                Stocks = addProduct.Stocks,
                Units = addProduct.Units,
                SupplierId = addProduct.SupplierId,
                CashierId = currentUser,        // ⭐ SECURED — linked to authenticated user
                BranchId = addProduct.BranchId  ,
                UserId = addProduct.UserId 
            };
            await _context.Products.AddAsync(product);
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
                Action = "Added",
                Description = $"Added product {addProduct.ProductName}",
                Date = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return "Successfully added products";
        }

        public async Task<string> UpdateProductOfSuppliers(string currentUser, string role, UpdateProductDTO update)
        {
            var product = await _context.Products
                .Where(p => p.Id == update.Id && p.CashierId == currentUser)   
                .FirstOrDefaultAsync();
            if (product == null)
                return "Product not found or access denied.";
            product.ProductName = update.ProductName ?? product.ProductName;
            product.CostPrice = update.Prices ?? product.CostPrice;
            product.Stocks = update.Stocks ?? product.Stocks;
            product.Units = update.Units ?? product.Units;
            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = currentUser,
                Role = role,
                Action = "Updated",
                Description = $"Updated product {product.ProductName}",
                Date = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return "Successfully updated products";
        }


        public async Task<bool> DeleteProductOfSuppliers(string currentUser, string role, int productId)
        {
            var product = await _context.Products
                .Where(p => p.Id == productId && p.CashierId == currentUser) 
                .FirstOrDefaultAsync();
            if (product == null)
                throw new Exception("Product not found or access denied.");
            _context.Products.Remove(product);
            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = currentUser,
                Role = role,
                Action = "Deleted",
                Description = $"Deleted product {product.ProductName}",
                Date = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return true;
        }

        
        public async Task<ICollection> GetAllProducts(string userID)
        {
            var products = await _context.Products
                .Where(p => p.UserId == userID)   // ⭐ CHANGE FROM CashierId TO UserId
                .Select(p => new
                {
                    p.Id,
                    p.ProductName,
                    p.Stocks,
                    p.Units,
                    p.CostPrice,
                    p.TransactionDate,
                    p.Supplier.SupplierName,

                    Branch = _context.Branches
                        .Where(b => b.Id == p.BranchId)
                        .Select(b => new
                        {
                            b.BranchName,
                            b.Location
                        })
                        .FirstOrDefault(),

                    Cashier = _context.UsersIdentity
                        .Where(c => c.Id == p.CashierId)
                        .Select(c => new
                        {
                            c.FirstName,
                            c.LastName,
                            c.Email
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();
            return products;
        }


        public async Task<ICollection> GetAllProducts_Admin()
        {
            var products = await _context.Products
                .Select(p => new
                {
                    p.Id,
                    p.ProductName,
                    p.Stocks,
                    p.Units,
                    p.CostPrice,
                    p.TransactionDate,
                    p.Supplier.SupplierName,
                    Branch = _context.Branches
                        .Where(b => b.Id == p.BranchId)
                        .Select(b => new
                        {
                            b.BranchName,
                            b.Location
                        })
                        .FirstOrDefault(),
                    Cashier = _context.UsersIdentity
                        .Where(c => c.Id == p.CashierId)
                        .Select(c => new
                        {
                            c.FirstName,
                            c.LastName,
                            c.Email
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();
            return products;
        }

        public async Task<ICollection> GetStockMovements(string userId)
        {
            var movements = await _context.StockMovements
                .Include(sm => sm.Product)
                .Where(sm => sm.UserId == userId)
                .OrderByDescending(sm => sm.TransactionDate)
                .Select(sm => new
                {
                    sm.Id,
                    sm.ProductId,
                    ProductName = sm.Product.ProductName,
                    sm.MovementType,
                    sm.Quantity,
                    sm.UnitPrice,
                    sm.Reason,
                    sm.TransactionDate,
                    sm.UserId,
                    sm.BranchId
                })
                .ToListAsync();
            return movements;
        }
        

    }
}
