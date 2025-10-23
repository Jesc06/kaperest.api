using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Users.Buy;
using KapeRest.Domain.Entities.MenuEntities;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructures.Persistence.Repositories.Users.Buy
{
    public class BuyRepo : IBuy
    {
        private readonly ApplicationDbContext _context;
        public BuyRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, object? Data)> BuyMenuItemAsync(int menuItemId)
        {
            var menuItem = await _context.MenuItems
       .Include(mi => mi.MenuItemProducts)
       .ThenInclude(mip => mip.ProductOfSupplier)
       .FirstOrDefaultAsync(mi => mi.Id == menuItemId);

            if (menuItem == null)
                return (false, "Menu item not found.", null);

            if (menuItem.MenuItemProducts == null || !menuItem.MenuItemProducts.Any())
                return (false, "No products linked to this menu item.", null);

            foreach (var mip in menuItem.MenuItemProducts)
            {
                var product = mip.ProductOfSupplier;

                if (product == null)
                    continue;

                if (product.Stock < mip.QuantityUsed)
                    return (false, $"Not enough stock for {product.ProductName}.", null);

                // 🧠 Make sure product is tracked before modifying
                _context.Attach(product);

                product.Stock -= mip.QuantityUsed;
            }

            await _context.SaveChangesAsync();

            return (true, $"Successfully bought {menuItem.ItemName}. Stock deducted.", new
            {
                MenuItem = menuItem.ItemName,
                UpdatedProducts = menuItem.MenuItemProducts.Select(m => new
                {
                    m.ProductOfSupplier.ProductName,
                    m.ProductOfSupplier.Stock
                })
            });
        }


    }
}
