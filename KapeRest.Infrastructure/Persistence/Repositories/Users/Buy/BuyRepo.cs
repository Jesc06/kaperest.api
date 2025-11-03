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
            // Get cashier info
            var cashier = await _context.UsersIdentity
                .FirstOrDefaultAsync(u => u.Id == buy.CashierId);

            if (cashier == null)
                return "Cashier not found";

            // Get menu item
            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts)
                    .ThenInclude(mp => mp.ProductOfSupplier)
                .FirstOrDefaultAsync(m => m.Id == buy.MenuItemId);

            if (menuItem == null)
                return "Menu item not found";

            // Deduct stocks (based on cashier)
            foreach (var itemProduct in menuItem.MenuItemProducts)
            {
                //Find the specific cashier’s product stock
                var product = await _context.Products
                    .FirstOrDefaultAsync(p =>
                        p.ProductName == itemProduct.ProductOfSupplier.ProductName &&
                        p.CashierId == buy.CashierId);

                if (product == null)
                    return $"No stock found for {itemProduct.ProductOfSupplier.ProductName} for this cashier.";

                var totalToDeduct = itemProduct.QuantityUsed * buy.Quantity;

                if (product.Stocks < totalToDeduct)
                    return $"Not enough stock for {product.ProductName} (Cashier: {cashier.UserName})";

                product.Stocks -= totalToDeduct;
            }

            // Compute totals
            decimal subtotal = menuItem.Price * buy.Quantity;
            decimal taxRate = buy.Tax / 100m;
            decimal discountRate = buy.DiscountPercent / 100m;
            decimal tax = subtotal * taxRate;
            decimal discount = subtotal * discountRate;
            decimal total = subtotal + tax - discount;

            // Save to SalesTransaction (linked to cashier + branch)
            var sale = new SalesTransactionEntities
            {
                CashierId = cashier.Id,
                BranchId = cashier.BranchId ?? 0,
                Subtotal = subtotal,
                Tax = tax,
                Discount = discount,
                Total = total,
                PaymentMethod = buy.PaymentMethod ?? "Cash",
                Status = "Completed",
            };

            _context.SalesTransaction.Add(sale);
            await _context.SaveChangesAsync();

            return $"Purchase successful (Receipt #{sale.ReceiptNumber})\nSubtotal:{subtotal}\nTax:{tax}\nDiscount:{discount}\nTotal:{total}";
        }

        public async Task<string> HoldTransaction(BuyMenuItemDTO buy) {
            var cashier = await _context.UsersIdentity
                    .FirstOrDefaultAsync(u => u.Id == buy.CashierId);

            if (cashier == null)
                return "Cashier not found";

            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == buy.MenuItemId);
            if (menuItem == null)
                return "Menu item not found";

            var subtotal = menuItem.Price * buy.Quantity;
            var tax = subtotal * (buy.Tax / 100m);
            var discount = subtotal * (buy.DiscountPercent / 100m);
            var total = subtotal + tax - discount;

            var transaction = new SalesTransactionEntities
            {
                CashierId = cashier.Id,
                BranchId = cashier.BranchId ?? 0,
                Subtotal = subtotal,
                Tax = tax,
                Discount = discount,
                Total = total,
                PaymentMethod = buy.PaymentMethod ?? "Cash",
                Status = "Hold"
            };

            _context.SalesTransaction.Add(transaction);
            await _context.SaveChangesAsync();

            // Save item details
            var saleItem = new SalesItemEntities
            {
                SalesTransactionId = transaction.Id,
                MenuItemId = menuItem.Id,
                Quantity = buy.Quantity,
                UnitPrice = menuItem.Price
            };
            _context.SalesItems.Add(saleItem);
            await _context.SaveChangesAsync();

            return $"Transaction held (Hold #{transaction.Id})";
        }


        public async Task<string> UpdateHeldTransaction(UpdateHoldTransaction update)
        {
            var transaction = await _context.SalesTransaction
             .FirstOrDefaultAsync(t => t.Id == update.SalesTransactionID && t.Status == "Hold");

            if (transaction == null)
                return "Held transaction not found";

            var saleItem = await _context.SalesItems
                .FirstOrDefaultAsync(s => s.SalesTransactionId == transaction.Id);

            if (saleItem == null)
                return "Sale item not found for this transaction";

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == saleItem.MenuItemId);

            if (menuItem == null)
                return "Menu item not found";

            // Recalculate
            var subtotal = menuItem.Price * update.Quantity;
            var tax = subtotal * (update.Tax / 100m);
            var discount = subtotal * (update.DiscountPercent / 100m);
            var total = subtotal + tax - discount;

            // Update transaction fields
            transaction.Subtotal = subtotal;
            transaction.Tax = tax;
            transaction.Discount = discount;
            transaction.Total = total;
            transaction.PaymentMethod = update.PaymentMethod ?? transaction.PaymentMethod;

            // Update sale item
            saleItem.Quantity = update.Quantity;
            saleItem.UnitPrice = menuItem.Price;

            await _context.SaveChangesAsync();

            return $"Held transaction #{transaction.Id} updated successfully";
        }


        public async Task<string> ResumeHoldAsync(int saleId)
        {
            //Get the sale info
            var sale = await _context.SalesTransaction
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null)
                return "Hold not found";

            if (sale.Status != "Hold")
                return "Already finalized";

            //Get cashier for this sale
            var cashier = await _context.UsersIdentity
                .FirstOrDefaultAsync(u => u.Id == sale.CashierId);

            if (cashier == null)
                return "Cashier not found";

            //Get sale items with menu items + product links
            var saleItems = await _context.SalesItems
                .Where(i => i.SalesTransactionId == sale.Id)
                .Include(i => i.MenuItem)
                    .ThenInclude(m => m.MenuItemProducts)
                        .ThenInclude(mp => mp.ProductOfSupplier)
                .ToListAsync();

            //Deduct stocks for each product (specific to cashier)
            foreach (var item in saleItems)
            {
                foreach (var menuItemProduct in item.MenuItem.MenuItemProducts)
                {
                    var productName = menuItemProduct.ProductOfSupplier.ProductName;
                    var totalToDeduct = menuItemProduct.QuantityUsed * item.Quantity;

                    //Find the product stock owned by this cashier
                    var cashierProduct = await _context.Products
                        .FirstOrDefaultAsync(p =>
                            p.ProductName == productName &&
                            p.CashierId == cashier.Id);

                    if (cashierProduct == null)
                        return $"No stock record found for {productName} (Cashier: {cashier.UserName})";

                    if (cashierProduct.Stocks < totalToDeduct)
                        return $"Not enough stock for {productName} (Cashier: {cashier.UserName})";

                    cashierProduct.Stocks -= totalToDeduct;
                }
            }
            //Mark sale as completed
            sale.Status = "Completed";
            await _context.SaveChangesAsync();

            return $"Hold transaction #{sale.Id} finalized successfully.";
        }


        public async Task<string> VoidItemAsync(int saleId)
        {
            var sale = await _context.SalesTransaction
            .Include(s => s.SalesItems)
            .ThenInclude(i => i.MenuItem)
            .ThenInclude(m => m.MenuItemProducts)
            .ThenInclude(mp => mp.ProductOfSupplier)
            .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null)
                return "Sale not found";

            if (sale.Status == "Voided")
                return "Already voided";

            var cashier = await _context.UsersIdentity.FirstOrDefaultAsync(u => u.Id == sale.CashierId);
            if (cashier == null)
                return "Cashier not found";

            foreach (var item in sale.SalesItems)
            {
                foreach (var menuItemProduct in item.MenuItem.MenuItemProducts)
                {
                    var productName = menuItemProduct.ProductOfSupplier.ProductName;
                    var totalToReturn = menuItemProduct.QuantityUsed * item.Quantity;

                    // Hanapin yung product stock ng cashier
                    var cashierProduct = await _context.Products
                        .FirstOrDefaultAsync(p =>
                            p.ProductName == productName &&
                            p.CashierId == cashier.Id);

                    if (cashierProduct != null)
                    {
                        // Ibalik yung stock
                        cashierProduct.Stocks += totalToReturn;
                    }
                }
            }

            sale.Status = "Voided";
            await _context.SaveChangesAsync();

            return $"Sale #{sale.Id} voided successfully, stocks restored.";
        }


        public async Task<string> CancelHoldAsync(int saleId)
        {
            var sale = await _context.SalesTransaction.FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) return "Hold not found";

            sale.Status = "Canceled";
            await _context.SaveChangesAsync();

            return $"Hold transaction #{sale.Id} canceled.";
        }



    }
}
