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
                Price = dto.Price,
                Description = dto.Description,
                ImagePath = dto.image
            };

            _context.MenuItems.Add(menuItem);
            await _context.SaveChangesAsync();

            if (dto.Products != null && dto.Products.Any())
            {
                foreach (var p in dto.Products)
                {
                    var link = new MenuItemProduct
                    {
                        MenuItemId = menuItem.Id,
                        ProductOfSupplierId = p.ProductId,
                        QuantityUsed = p.QuantityUsed
                    };
                    _context.MenuItemProducts.Add(link);
                }

                await _context.SaveChangesAsync();
            }

            return menuItem;
        }


    }
}
