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

        public async Task<bool> BuyMenuItemAsync(int menuItemId)
        {
            var menuItem = await _context.MenuItems
               .Include(m => m.MenuItemProducts)
               .ThenInclude(mp => mp.ProductOfSupplier)
               .FirstOrDefaultAsync(m => m.Id == menuItemId);

            if (menuItem == null) return false;

            foreach (var mp in menuItem.MenuItemProducts)
            {
                mp.ProductOfSupplier.Stock -= mp.QuantityUsed;
            }

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
