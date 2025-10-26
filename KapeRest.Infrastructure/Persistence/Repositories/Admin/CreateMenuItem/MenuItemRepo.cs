using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using KapeRest.Application.Interfaces.Admin.CreateMenuItem;
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
    public class MenuItemRepo : IMenuItem
    {

        private readonly ApplicationDbContext _context;
        public MenuItemRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<MenuItem> CreateMenuItemAsync(string user, string role, CreateMenuItemDTO dto)
        {
            var menuItem = new MenuItem
            {
                ItemName = dto.Item_name,
                Price = dto.Price,
                Description = dto.Description,
                Image = dto.Image
            };

            // Auto-link menu item to its products
            foreach (var product in dto.Products)
            {
                menuItem.MenuItemProducts.Add(new MenuItemProduct
                {
                    ProductOfSupplierId = product.ProductOfSupplierId,
                    QuantityUsed = product.QuantityUsed
                });
            }

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return menuItem;
        }

        public async Task<MenuItem> UpdateMenuItemAsync(int menuItemId, UpdateMenuItemDTO dto)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts) 
                .FirstOrDefaultAsync(m => m.Id == menuItemId);

            if (menuItem == null)
                throw new KeyNotFoundException("Menu item not found");

            menuItem.ItemName = dto.Item_name;
            menuItem.Price = dto.Price;
            menuItem.Description = dto.Description;
            menuItem.Image = dto.Image;

            menuItem.MenuItemProducts.Clear();

            foreach (var product in dto.Products)
            {
                menuItem.MenuItemProducts.Add(new MenuItemProduct
                {
                    ProductOfSupplierId = product.ProductOfSupplierId,
                    QuantityUsed = product.QuantityUsed
                });
            }

            await _context.SaveChangesAsync();

            return menuItem;
        }
        public async Task<string> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (menuItem == null)
                return "Menu item not found";

            _context.MenuItems.Remove(menuItem);

            await _context.SaveChangesAsync();

            return "Successfully deleted menu item";
        }

        public async Task<ICollection> GetAllMenuItem()
        {
            var menuItems = await _context.MenuItems
                .Include(m => m.MenuItemProducts)
                .ToListAsync();
            return menuItems;
        }


    }
}
