using KapeRest.Application.DTOs.Admin.CreateMenuItem;
using KapeRest.Application.Interfaces.Admin.CreateMenuItem;
using KapeRest.Domain.Entities.AuditLogEntities;
using KapeRest.Domain.Entities.MenuEntities;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
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



        public async Task<MenuItem> CreateMenuItem(string currentUser, string role, CreateMenuItemDTO dto)
        {
            // Create entity
            var menuItem = new MenuItem
            {
                Name = dto.ItemName,
                Price = dto.Price
            };

            // Validate & Deduct
            var productIds = dto.Ingredients.Select(i => i.ProductId).ToList();
            var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

            if (products.Count != dto.Ingredients.Count)
                throw new Exception("One or more ingredient products not found.");

            foreach (var ingredient in dto.Ingredients)
            {
                var product = products.First(p => p.Id == ingredient.ProductId);
                if (product.Stock < ingredient.Quantity)
                    throw new Exception($"Not enough stock for {product.ProductName}");

                product.Stock -= ingredient.Quantity;

                // Log action
                _context.AuditLog.Add(new AuditLogEntities
                {
                    User = currentUser,
                    Role = role,
                    Category = "Inventory",
                    Action = "Deduct",
                    AffectedEntity = product.ProductName,
                    Description = $"Used {ingredient.Quantity} of {product.ProductName} for {menuItem.Name}",
                    Date = DateTime.Now
                });

                _context.MenuItemProducts.Add(new MenuItemProduct
                {
                    MenuItem = menuItem,
                    Product = product,
                    Quantity = ingredient.Quantity
                });
            }

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();
            return menuItem;
        }


    }
}
