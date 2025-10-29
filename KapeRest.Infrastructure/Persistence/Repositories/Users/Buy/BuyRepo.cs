using Azure.Core;
using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Users.Buy;
using KapeRest.Core.Entities.SalesTransaction;
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
            //Get cashier info
            var cashier = await _context.UsersIdentity
                .FirstOrDefaultAsync(u => u.Id == buy.CashierId);

            if (cashier == null)
                return "Cashier not found";

            //Get menu item
            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts)
                    .ThenInclude(mp => mp.ProductOfSupplier)
                .FirstOrDefaultAsync(m => m.Id == buy.MenuItemId);

            if (menuItem == null)
                return "Menu item not found";

            //Deduct stock
            foreach (var itemProduct in menuItem.MenuItemProducts)
            {
                var product = itemProduct.ProductOfSupplier;
                var totalToDeduct = itemProduct.QuantityUsed * buy.Quantity;

                if (product.Stocks < totalToDeduct)
                    return $"Not enough stock for {product.ProductName}";

                product.Stocks -= totalToDeduct;
            }

            //Compute totals
            decimal subtotal = menuItem.Price * buy.Quantity;

            // Convert the raw percent values into decimals
            decimal taxRate = buy.Tax / 100m;
            decimal discountRate = buy.DiscountPercent / 100m;

            decimal tax = subtotal * taxRate;
            decimal discount = subtotal * discountRate;
            decimal total = subtotal + tax - discount;

            //Save to SalesTransaction (linked to cashier + branch)
            var sale = new SalesTransactionEntities
            {
                CashierId = cashier.Id,
                BranchId = cashier.BranchId ?? 0,
                Subtotal = subtotal,
                Tax = tax,
                Discount = discount,
                Total = total,
                PaymentMethod = buy.PaymentMethod ?? "Cash"
            };

            _context.SalesTransaction.Add(sale);
            await _context.SaveChangesAsync();

            return $"Purchase successful (Receipt #{sale.ReceiptNumber})\nSubtotal:{subtotal}\nTax:{tax}\nDiscount:{discount}\nTotal:{total}";
        }



    }
}
