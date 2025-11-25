using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using KapeRest.Application.Interfaces.Admin.CreateMenuItem;
using KapeRest.Core.Entities.MenuEntities;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.MenuEntities;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructures.Persistence.Repositories.Admin.CreateMenuItem
{
    public class MenuItemRepository : IMenuItem
    {
        private readonly ApplicationDbContext _context;
        public MenuItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper: determine availability based on linked products and their stocks
        private string CheckAvailability(MenuItem menuItem)
        {
            if (menuItem.MenuItemProducts == null || menuItem.MenuItemProducts.Count == 0)
            {
                // If there are no linked products, consider it Available by default
                return "Available";
            }

            foreach (var itemProduct in menuItem.MenuItemProducts)
            {
                var product = itemProduct.ProductOfSupplier;
                if (product == null)
                {
                    // missing product -> out of stock
                    return "Out of Stock";
                }

                if (product.Stocks <= 0)
                {
                    return "Out of Stock";
                }

                if (product.Stocks < itemProduct.QuantityUsed)
                {
                    return "Out of Stock";
                }
            }

            return "Available";
        }

        public async Task<MenuItem> CreateMenuItemAsync(string user, string role, CreateMenuItemDTO dto)
        {
            var menuItem = new MenuItem
            {
                ItemName = dto.Item_name,
                Price = dto.Price,
                Category = dto.Category,
                Description = dto.Description,
                Image = dto.Image,
                IsAvailable = dto.IsAvailable,
                CashierId = dto.cashierId,
                BranchId = null
            };

            // Only validate products if there are any
            if (dto.Products != null && dto.Products.Count > 0)
            {
                foreach (var product in dto.Products)
                {
                    var productExists = await _context.Products
                        .AnyAsync(p => p.Id == product.ProductOfSupplierId);

                    if (!productExists)
                        throw new Exception($"Product {product.ProductOfSupplierId} not found");
                    menuItem.MenuItemProducts.Add(new MenuItemProduct
                    {
                        ProductOfSupplierId = product.ProductOfSupplierId,
                        QuantityUsed = product.QuantityUsed
                    });
                }
            }

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return menuItem;
        }

        public async Task<MenuItem> UpdateMenuItemAsync(UpdateMenuItemDTO dto)
        {
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == dto.Id && m.CashierId == dto.cashierId);

            if (menuItem == null)
                throw new KeyNotFoundException("Menu item not found or does not belong to this cashier");

            // Update fields
            menuItem.ItemName = dto.Item_name;
            menuItem.Price = dto.Price;
            menuItem.Category = dto.Category;
            menuItem.Description = dto.Description;
            menuItem.Image = dto.Image;
            menuItem.IsAvailable = dto.IsAvailable;

            // Remove existing MenuItemProducts
            var existingProducts = _context.MenuItemProducts
                .Where(mp => mp.MenuItemId == dto.Id);
            _context.MenuItemProducts.RemoveRange(existingProducts);

            // Add new products (validation removed - products are shared)
            foreach (var product in dto.Products)
            {
                _context.MenuItemProducts.Add(new MenuItemProduct
                {
                    MenuItemId = dto.Id,
                    ProductOfSupplierId = product.ProductOfSupplierId,
                    QuantityUsed = product.QuantityUsed
                });
            }

            await _context.SaveChangesAsync();
            return menuItem;
        }


        public async Task<string> DeleteMenuItem(string cashierId, int id)
        {
            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == id && m.CashierId == cashierId);

            if (menuItem == null)
                return "Menu item not found or does not belong to this cashier";

            // Delete child MenuItemProducts only
            var linkedProducts = _context.MenuItemProducts
                .Where(mp => mp.MenuItemId == id);
            _context.MenuItemProducts.RemoveRange(linkedProducts);

            // SalesItems are not deleted; MenuItemId will be set to NULL automatically
            _context.MenuItems.Remove(menuItem);

            try
            {
                await _context.SaveChangesAsync();
                return "Successfully deleted menu item";
            }
            catch (DbUpdateException ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
        }




        public async Task<ICollection> GetAllMenuItem(string cashierId)
        {
            var menuItems = await _context.MenuItems
                .Where(m => m.CashierId == cashierId)
                .Include(m => m.MenuItemProducts)
                    .ThenInclude(mp => mp.ProductOfSupplier)
                .ToListAsync();

            foreach (var menuItem in menuItems)
            {
                // Compute availability
                string newStatus = CheckAvailability(menuItem);

                // Update only if changed (prevents unnecessary DB operations)
                if (menuItem.IsAvailable != newStatus)
                {
                    menuItem.IsAvailable = newStatus;
                    _context.MenuItems.Update(menuItem);
                }
            }

            await _context.SaveChangesAsync();
            return menuItems;
        }




    }
}
