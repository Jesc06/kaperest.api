using Azure.Core;
using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Cashiers.Buy;
using KapeRest.Core.Entities.SalesTransaction;
using KapeRest.Domain.Entities.MenuEntities;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Infrastructures.Persistence.Repositories.Cashiers.Buy
{
    public class BuyRepository : IBuy
    {
        private readonly ApplicationDbContext _context;
        public BuyRepository(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<string> BuyMenuItemAsync(BuyMenuItemDTO buy)
        {
            var cashier = await _context.UsersIdentity.FirstOrDefaultAsync(u => u.Id == buy.CashierId);
            if (cashier == null) throw new Exception("Cashier not found");

            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts)
                    .ThenInclude(mp => mp.ProductOfSupplier)
                .FirstOrDefaultAsync(m => m.Id == buy.MenuItemId);
            if (menuItem == null) throw new Exception("Menu item not found");

            foreach (var itemProduct in menuItem.MenuItemProducts)
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == itemProduct.ProductOfSupplierId);
                if (product == null) throw new Exception($"Product {itemProduct.ProductOfSupplier.ProductName} not found in inventory.");

                var totalToDeduct = itemProduct.QuantityUsed * buy.Quantity;
                if (product.Stocks < totalToDeduct)
                    throw new Exception($"Insufficient stock for {product.ProductName}. Available: {product.Stocks}, Required: {totalToDeduct}");

                product.Stocks -= totalToDeduct;
                _context.Products.Update(product);
            }

            decimal subtotal = menuItem.Price * buy.Quantity;
            decimal tax = subtotal * (buy.Tax / 100m);
            decimal discount = subtotal * (buy.DiscountPercent / 100m);
            decimal total = subtotal + tax - discount;

            var sale = new SalesTransactionEntities
            {
                MenuItemName = menuItem.ItemName,
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
            await _context.SaveChangesAsync(); // ✅ Save first to get sale.Id

            // ✅ ADD THIS: Save SalesItem details
            var saleItem = new SalesItemEntities
            {
                SalesTransactionId = sale.Id,
                MenuItemId = menuItem.Id,
                Quantity = buy.Quantity,
                UnitPrice = menuItem.Price
            };
            _context.SalesItems.Add(saleItem);
            await _context.SaveChangesAsync(); // ✅ Save SalesItem

            return $"Purchase successful (Receipt #{sale.MenuItemName})\nSubtotal: ₱{subtotal:F2}\nTax: ₱{tax:F2}\nDiscount: ₱{discount:F2}\nTotal: ₱{total:F2}";
        }

        public async Task<string> HoldTransaction(BuyMenuItemDTO buy)
        {
            var cashier = await _context.UsersIdentity.FirstOrDefaultAsync(u => u.Id == buy.CashierId);
            if (cashier == null) throw new Exception("Cashier not found");

            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts)
                    .ThenInclude(mp => mp.ProductOfSupplier)
                .FirstOrDefaultAsync(m => m.Id == buy.MenuItemId);
            if (menuItem == null) throw new Exception("Menu item not found");

            var subtotal = menuItem.Price * buy.Quantity;
            var tax = subtotal * (buy.Tax / 100m);
            var discount = subtotal * (buy.DiscountPercent / 100m);
            var total = subtotal + tax - discount;

            var transaction = new SalesTransactionEntities
            {
                MenuItemName = menuItem.ItemName,
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

            var saleItem = new SalesItemEntities
            {
                SalesTransactionId = transaction.Id,
                MenuItemId = menuItem.Id,
                Quantity = buy.Quantity,
                UnitPrice = menuItem.Price
            };
            _context.SalesItems.Add(saleItem);
            await _context.SaveChangesAsync();

            return $"Transaction held (Hold #{transaction.Id})\nSubtotal: ₱{subtotal:F2}\nTax: ₱{tax:F2}\nDiscount: ₱{discount:F2}\nTotal: ₱{total:F2}";
        }

        public async Task<string> UpdateHeldTransaction(UpdateHoldTransaction update)
        {
            var transaction = await _context.SalesTransaction
                .FirstOrDefaultAsync(t => t.Id == update.SalesTransactionID && t.Status == "Hold");
            if (transaction == null) return "Held transaction not found";

            var saleItem = await _context.SalesItems
                .FirstOrDefaultAsync(s => s.SalesTransactionId == transaction.Id);
            if (saleItem == null) return "Sale item not found for this transaction";

            var menuItem = await _context.MenuItems.FirstOrDefaultAsync(m => m.Id == saleItem.MenuItemId);
            if (menuItem == null) return "Menu item not found";

            var subtotal = menuItem.Price * update.Quantity;
            var tax = subtotal * (update.Tax / 100m);
            var discount = subtotal * (update.DiscountPercent / 100m);
            var total = subtotal + tax - discount;

            transaction.Subtotal = subtotal;
            transaction.Tax = tax;
            transaction.Discount = discount;
            transaction.Total = total;
            transaction.PaymentMethod = update.PaymentMethod ?? transaction.PaymentMethod;

            saleItem.Quantity = update.Quantity;
            saleItem.UnitPrice = menuItem.Price;

            await _context.SaveChangesAsync();

            return $"Held transaction #{transaction.Id} updated successfully";
        }

        public async Task<string> ResumeHoldAsync(int saleId)
        {
            var sale = await _context.SalesTransaction
                .FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) throw new Exception("Hold not found");
            if (sale.Status != "Hold") throw new Exception("Transaction already processed");

            var cashier = await _context.UsersIdentity
                .FirstOrDefaultAsync(u => u.Id == sale.CashierId);
            if (cashier == null) throw new Exception("Cashier not found");

            var saleItems = await _context.SalesItems
                .Where(i => i.SalesTransactionId == sale.Id)
                .Include(i => i.MenuItem)
                    .ThenInclude(m => m.MenuItemProducts)
                        .ThenInclude(mp => mp.ProductOfSupplier)
                .ToListAsync();

            foreach (var item in saleItems)
            {
                foreach (var menuItemProduct in item.MenuItem.MenuItemProducts)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == menuItemProduct.ProductOfSupplierId);
                    if (product == null)
                        throw new Exception($"Product {menuItemProduct.ProductOfSupplier.ProductName} not found in inventory.");

                    var totalToDeduct = menuItemProduct.QuantityUsed * item.Quantity;
                    if (product.Stocks < totalToDeduct)
                        throw new Exception($"Insufficient stock for {product.ProductName}. Available: {product.Stocks}, Required: {totalToDeduct}");

                    product.Stocks -= totalToDeduct;
                    _context.Products.Update(product);
                }
            }

            // Assign MenuItemName from items
            sale.MenuItemName = string.Join(", ", saleItems.Select(i => i.MenuItem.ItemName));
            sale.Status = "Completed";

            await _context.SaveChangesAsync();

            return $"Hold transaction #{sale.Id} completed successfully.";
        }

        public async Task<string> VoidItemAsync(int saleId)
        {
            var sale = await _context.SalesTransaction
                .Include(s => s.SalesItems)
                    .ThenInclude(i => i.MenuItem)
                        .ThenInclude(m => m.MenuItemProducts)
                            .ThenInclude(mp => mp.ProductOfSupplier)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return "Sale not found";
            if (sale.Status == "Voided") return "Already voided";

            foreach (var item in sale.SalesItems)
            {
                foreach (var menuItemProduct in item.MenuItem.MenuItemProducts)
                {
                    var product = await _context.Products
                        .FirstOrDefaultAsync(p => p.Id == menuItemProduct.ProductOfSupplierId);
                    if (product != null)
                    {
                        product.Stocks += menuItemProduct.QuantityUsed * item.Quantity;
                        _context.Products.Update(product);
                    }
                }
            }

            sale.Status = "Voided";
            await _context.SaveChangesAsync();

            return $"Sale #{sale.Id} voided successfully.";
        }

        public async Task<string> CancelHoldAsync(int saleId)
        {
            var sale = await _context.SalesTransaction.FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) return "Hold not found";

            sale.Status = "Canceled";
            await _context.SaveChangesAsync();

            return $"Hold transaction #{sale.Id} canceled.";
        }

        public async Task<ICollection> GetHoldTransactions(string cashierId)
        {
            var holdTransactions = await _context.SalesTransaction
                .Where(s => s.CashierId == cashierId && s.Status == "Hold")
                .Include(s => s.SalesItems)
                    .ThenInclude(i => i.MenuItem)
                .OrderByDescending(s => s.DateTime)
                .Select(s => new
                {
                    s.Id,
                    s.CashierId,
                    s.BranchId,
                    s.Subtotal,
                    s.Tax,
                    s.Discount,
                    s.Total,
                    s.PaymentMethod,
                    s.Status,
                    s.DateTime,
                    s.MenuItemName,
                    SalesItems = s.SalesItems.Select(item => new
                    {
                        item.Id,
                        item.SalesTransactionId,
                        item.MenuItemId,
                        item.Quantity,
                        item.UnitPrice,
                        MenuItem = new
                        {
                            item.MenuItem.Id,
                            item.MenuItem.ItemName,
                            item.MenuItem.Price,
                            item.MenuItem.Category,
                            item.MenuItem.Image
                        }
                    }).ToList()
                })
                .ToListAsync();

            return holdTransactions;
        }


    }


}
