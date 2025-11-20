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
                                .AnyAsync(p => p.Id == product.ProductOfSupplierId && p.CashierId == dto.cashierId);

                            if (!productExists)
                                throw new Exception($"Product {product.ProductOfSupplierId} does not belong to this cashier");

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

            // Add new products
            foreach (var product in dto.Products)
            {
                // Ensure product belongs to this cashier
                var exists = await _context.Products
                    .AnyAsync(p => p.Id == product.ProductOfSupplierId && p.CashierId == dto.cashierId);

                if (!exists)
                    throw new Exception($"Product {product.ProductOfSupplierId} does not belong to this cashier");

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

            // Delete linked MenuItemProducts
            var linkedProducts = _context.MenuItemProducts
                .Where(mp => mp.MenuItemId == id);
            _context.MenuItemProducts.RemoveRange(linkedProducts);

            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return "Successfully deleted menu item";
        }

        public async Task<ICollection> GetAllMenuItem(string cashierId)
        {
            var menuItems = await _context.MenuItems
           .Where(m => m.CashierId == cashierId)
           .ToListAsync();

            // Optional: load MenuItemProducts separately if needed
            var menuItemIds = menuItems.Select(m => m.Id).ToList();
            var allProducts = await _context.MenuItemProducts
                .Where(mp => menuItemIds.Contains(mp.MenuItemId))
                .ToListAsync();

            // Assign products manually (if you want to return it in one object)
            foreach (var menuItem in menuItems)
            {
                menuItem.MenuItemProducts = allProducts
                    .Where(mp => mp.MenuItemId == menuItem.Id)
                    .ToList();
            }

            return menuItems;
        }


    }
}

