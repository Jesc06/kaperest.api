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

        public async Task<MenuItem> CreateMenuItemAsync(string user, string role, CreateMenuItemDTO dto)
        {
            var menuItem = new MenuItem
            {
                ItemName = dto.ItemName,
                Price = dto.Price
            };

            foreach (var productDto in dto.Products)
            {
                var product = await _context.Products.FindAsync(productDto.ProductId);
                if (product == null)
                    throw new Exception($"Product ID {productDto.ProductId} not found.");

                if (product.Stock < productDto.QuantityUsed)
                    throw new Exception($"Not enough stock for product: {product.ProductName}");
                    
                if (product.Stock <= 0)
                    throw new Exception("No available stock");

                product.Stock -= productDto.QuantityUsed;

                menuItem.MenuItemProducts.Add(new MenuItemProduct
                {
                    ProductId = product.Id,
                    Quantity = productDto.QuantityUsed
                });
            }

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            return menuItem;
        }


    }
}
