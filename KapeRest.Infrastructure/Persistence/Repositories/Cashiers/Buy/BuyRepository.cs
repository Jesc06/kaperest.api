using Azure.Core;
using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Cashiers.Buy;
using KapeRest.Core.Entities.SalesTransaction;
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


            string generatedReceipt = $"RCP-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            var sale = new SalesTransactionEntities
            {
                ReceiptNumber = generatedReceipt,
                MenuItemName = menuItem.ItemName,
                CashierId = cashier.Id,
                BranchId = cashier.BranchId,
                Subtotal = subtotal,
                Tax = tax,
                Discount = discount,
                Total = total,
                PaymentMethod = buy.PaymentMethod ?? "Cash",
                Status = "Completed",
                Reason = null
            };

            _context.SalesTransaction.Add(sale);
            await _context.SaveChangesAsync(); //Save first to get sale.Id

            //Save SalesItem details
            var saleItem = new SalesItemEntities
            {
                SalesTransactionId = sale.Id,
                MenuItemId = menuItem.Id,
                Quantity = buy.Quantity,
                UnitPrice = menuItem.Price
            };
            _context.SalesItems.Add(saleItem);

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = cashier.Email ?? cashier.UserName ?? "Unknown",
                Role = "Cashier",
                Action = "Purchase",
                Description = $"Completed purchase of {menuItem.ItemName} (Qty: {buy.Quantity}, Total: ₱{total:F2})",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync(); //Save SalesItem

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
                BranchId = cashier.BranchId,
                Subtotal = subtotal,
                Tax = tax,
                Discount = discount,
                Total = total,
                PaymentMethod = buy.PaymentMethod ?? "Cash",
                Status = "Hold",
                Reason = null
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

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = cashier.Email ?? cashier.UserName ?? "Unknown",
                Role = "Cashier",
                Action = "Hold Transaction",
                Description = $"Put transaction on hold for {menuItem.ItemName} (Qty: {buy.Quantity})",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return $"Transaction held (Hold #{transaction.Id})\nSubtotal: ₱{subtotal:F2}\nTax: ₱{tax:F2}\nDiscount: ₱{discount:F2}\nTotal: ₱{total:F2}";
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

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = cashier.Email ?? cashier.UserName ?? "Unknown",
                Role = "Cashier",
                Action = "Resume Hold",
                Description = $"Resumed and completed hold transaction #{sale.Id}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return $"Hold transaction #{sale.Id} completed successfully.";
        }

        public async Task<string> VoidItemAsync(int saleId, string userId, string role)
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

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = userId,
                Role = role,
                Action = "Void Sale",
                Description = $"Voided sale #{sale.Id}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return $"Sale #{sale.Id} voided successfully.";
        }

        //Void Request to staff
        public async Task<string> RequestVoidAsync(int saleId, string reason,string user, string role)
        {
            var sale = await _context.SalesTransaction
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return "Sale not found";
            if (sale.Status == "Voided") return "Sale already voided";
            if (sale.Status == "PendingVoid") return "Void request already pending";

            sale.Status = "PendingVoid";
            sale.Reason = reason;

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = user,
                Role = role,
                Action = "Request Void",
                Description = $"Requested void for sale #{sale.Id}. Reason: {reason}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return $"Void request submitted for Sale #{sale.Id}. Awaiting admin approval.";
        }


        public async Task<string> ApproveVoidAsync(int saleId, string user, string role)
        {
            var sale = await _context.SalesTransaction
                .Include(s => s.SalesItems)
                    .ThenInclude(i => i.MenuItem)
                        .ThenInclude(m => m.MenuItemProducts)
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return "Sale not found";
            if (sale.Status != "PendingVoid") return "Sale is not pending void";

            // Return stocks
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

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = user,
                Role = role,
                Action = "Approve Void",
                Description = $"Approved void request for sale #{sale.Id}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return $"Sale #{sale.Id} has been voided successfully.";
        }



        public async Task<string> RejectVoidAsync(int saleId, string userId, string role)
        {
            var sale = await _context.SalesTransaction
                .FirstOrDefaultAsync(s => s.Id == saleId);

            if (sale == null) return "Sale not found";
            if (sale.Status != "PendingVoid") return "Sale is not pending void";

            sale.Status = "Completed";

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = userId,
                Role = role,
                Action = "Reject Void",
                Description = $"Rejected void request for sale #{sale.Id}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return $"Void request for Sale #{sale.Id} has been rejected.";
        }





        public async Task<string> CancelHoldAsync(int saleId)
        {
            var sale = await _context.SalesTransaction.FirstOrDefaultAsync(s => s.Id == saleId);
            if (sale == null) return "Hold not found";

            sale.Status = "Canceled";

            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = sale.CashierId,
                Role = "Cashier",
                Action = "Cancel Hold",
                Description = $"Canceled hold transaction #{sale.Id}",
                Date = DateTime.Now
            });

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
