using Azure.Core;
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

        public async Task<string> BuyMenuItemAsync(BuyMenuItemDTO buy)
        {
            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts)
                    .ThenInclude(mp => mp.ProductOfSupplier)
                .FirstOrDefaultAsync(m => m.Id == buy.MenuItemId);

            if (menuItem == null)
                return "Menu item not found";

            //Deduct stock from each related product
            foreach (var itemProduct in menuItem.MenuItemProducts)
            {
                var product = itemProduct.ProductOfSupplier;
                var totalToDeduct = itemProduct.QuantityUsed * buy.Quantity;

                if (product.Stocks < totalToDeduct)
                {
                    var IsAvailable = _context.MenuItems.FirstOrDefault(m => m.Id == buy.MenuItemId);
                    IsAvailable.IsAvailable = "Not yet";
                    _context.MenuItems.Update(IsAvailable);
                    await _context.SaveChangesAsync();
                    return $"Not enough stock for product '{product.ProductName}'. Available: {product.Stocks}, needed: {totalToDeduct}";
                }
                var Available = _context.MenuItems.FirstOrDefault(m => m.Id == buy.MenuItemId);
                Available.IsAvailable = "Yes";
                _context.MenuItems.Update(Available);
                await _context.SaveChangesAsync();
                product.Stocks -= totalToDeduct;
            }

            await _context.SaveChangesAsync();

            return "Purchase successful";
        }



    }
}
