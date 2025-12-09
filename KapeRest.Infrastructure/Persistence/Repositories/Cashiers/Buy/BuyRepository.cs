using Azure.Core;
using KapeRest.Application.DTOs.PayMongo;
using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Cashiers.Buy;
using KapeRest.Application.Interfaces.PayMongo;
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
        private readonly IPayMongo _payMongo;
        public BuyRepository(ApplicationDbContext context, IPayMongo payMongo)
        {
            _context = context;
            _payMongo = payMongo;
        }


        public async Task<string> BuyMenuItemAsync(BuyMenuItemDTO buy)
        {
            var cashier = await _context.UsersIdentity.FirstOrDefaultAsync(u => u.Id == buy.CashierId);
            if (cashier == null) throw new Exception("Cashier not found");

            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts)
                    .ThenInclude(mp => mp.ProductOfSupplier)
                .Include(m => m.MenuItemSizes)
                .FirstOrDefaultAsync(m => m.Id == buy.MenuItemId);
            if (menuItem == null) throw new Exception("Menu item not found");

            // Determine the price based on size
            decimal itemPrice = menuItem.Price;
            int? sizeId = null;
            string selectedSize = "Regular";

            if (!string.IsNullOrEmpty(buy.Size) && menuItem.MenuItemSizes != null && menuItem.MenuItemSizes.Any())
            {
                var sizeVariation = menuItem.MenuItemSizes.FirstOrDefault(s => s.Size.Equals(buy.Size, StringComparison.OrdinalIgnoreCase));
                if (sizeVariation != null)
                {
                    itemPrice = sizeVariation.Price;
                    sizeId = sizeVariation.Id;
                    selectedSize = sizeVariation.Size;
                }
            }
            else if (buy.MenuItemSizeId.HasValue)
            {
                var sizeVariation = menuItem.MenuItemSizes?.FirstOrDefault(s => s.Id == buy.MenuItemSizeId.Value);
                if (sizeVariation != null)
                {
                    itemPrice = sizeVariation.Price;
                    sizeId = sizeVariation.Id;
                    selectedSize = sizeVariation.Size;
                }
            }

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

            decimal subtotal = itemPrice * buy.Quantity;
            decimal tax = subtotal * (buy.Tax / 100m);
            decimal discount = subtotal * (buy.DiscountPercent / 100m);
            decimal total = subtotal + tax - discount;


            string gcashCheckoutUrl = null;
            string paymentReference = null;

            if (buy.PaymentMethod?.ToLower() == "gcash")
            {
                var payment = await _payMongo.CreateGcashPaymentAsync(new CreatePaymentDTO
                {
                    Amount = total
                });

                gcashCheckoutUrl = payment.CheckoutUrl;
                paymentReference = payment.ReferenceId;

                // Save pending payment data for later completion
                await _payMongo.SavePendingPaymentAsync(new PendingPaymentDTO
                {
                    PaymentReference = paymentReference,
                    CashierId = cashier.Id,
                    BranchId = cashier.BranchId,
                    CartItems = new List<CartItemDTO>
                    {
                        new CartItemDTO
                        {
                            MenuItemId = menuItem.Id,
                            MenuItemSizeId = sizeId,
                            Size = selectedSize,
                            Quantity = buy.Quantity,
                            Price = itemPrice,
                            SugarLevel = buy.SugarLevel ?? "100%"
                        }
                    },
                    DiscountPercent = buy.DiscountPercent,
                    TaxPercent = buy.Tax,
                    TotalAmount = total
                });
            }


            // For GCash payments, don't save transaction yet - wait for webhook completion
            if (buy.PaymentMethod?.ToLower() == "gcash")
            {
                return
                    $"GCASH Payment Generated!\n" +
                    $"Total: ₱{total:F2}\n\n" +
                    $"Pay here:\n{gcashCheckoutUrl}\n\n" +
                    $"Reference ID: {paymentReference}";
            }

            // For non-GCash payments, save transaction normally
            // Use Philippine Time for consistency with sales reports
            var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var philippineNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone);
            
            var sale = new SalesTransactionEntities
            {
                ReceiptNumber = $"REC{philippineNow.Year}{philippineNow:MMddHHmmss}",
                MenuItemName = menuItem.ItemName,
                DateTime = philippineNow,
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

            //Save SalesItem details with size information
            var saleItem = new SalesItemEntities
            {
                SalesTransactionId = sale.Id,
                MenuItemId = menuItem.Id,
                MenuItemSizeId = sizeId,
                Size = selectedSize,
                SugarLevel = buy.SugarLevel ?? "100%", // Default to 100% if not specified
                Quantity = buy.Quantity,
                UnitPrice = itemPrice
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




        public async Task UpdatePaymentStatusAsync(string paymentReference, string status)
        {
            var sale = await _context.SalesTransaction
                .FirstOrDefaultAsync(s => s.PaymentReference == paymentReference);

            if (sale == null)
                throw new Exception($"Sale with PaymentReference {paymentReference} not found.");

            // Update status based on PayMongo status
            // PayMongo statuses: 'paid', 'failed', 'canceled', etc.
            switch (status.ToLower())
            {
                case "paid":
                    sale.Status = "Completed";
                    break;
                case "failed":
                case "canceled":
                    sale.Status = "Failed";
                    break;
                default:
                    sale.Status = "Completed";
                    break;
            }

            _context.SalesTransaction.Update(sale);

            // Optional: Add audit log
            _context.AuditLog.Add(new AuditLogEntities
            {
                Username = sale.CashierId,
                Role = "System",
                Action = "Payment Webhook",
                Description = $"Payment status for Sale #{sale.Id} updated to {sale.Status} via PayMongo webhook.",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }





        public async Task<string> HoldTransaction(BuyMenuItemDTO buy)
        {
            var cashier = await _context.UsersIdentity.FirstOrDefaultAsync(u => u.Id == buy.CashierId);
            if (cashier == null) throw new Exception("Cashier not found");

            var menuItem = await _context.MenuItems
                .Include(m => m.MenuItemProducts)
                    .ThenInclude(mp => mp.ProductOfSupplier)
                .Include(m => m.MenuItemSizes)
                .FirstOrDefaultAsync(m => m.Id == buy.MenuItemId);
            if (menuItem == null) throw new Exception("Menu item not found");

            // Determine the price based on size
            decimal itemPrice = menuItem.Price;
            int? sizeId = null;
            string selectedSize = "Regular";

            if (!string.IsNullOrEmpty(buy.Size) && menuItem.MenuItemSizes != null && menuItem.MenuItemSizes.Any())
            {
                var sizeVariation = menuItem.MenuItemSizes.FirstOrDefault(s => s.Size.Equals(buy.Size, StringComparison.OrdinalIgnoreCase));
                if (sizeVariation != null)
                {
                    itemPrice = sizeVariation.Price;
                    sizeId = sizeVariation.Id;
                    selectedSize = sizeVariation.Size;
                }
            }
            else if (buy.MenuItemSizeId.HasValue)
            {
                var sizeVariation = menuItem.MenuItemSizes?.FirstOrDefault(s => s.Id == buy.MenuItemSizeId.Value);
                if (sizeVariation != null)
                {
                    itemPrice = sizeVariation.Price;
                    sizeId = sizeVariation.Id;
                    selectedSize = sizeVariation.Size;
                }
            }

            decimal subtotal = itemPrice * buy.Quantity;
            decimal tax = subtotal * (buy.Tax / 100m);
            decimal discount = subtotal * (buy.DiscountPercent / 100m);
            decimal total = subtotal + tax - discount;

            // Use Philippine Time for consistency
            var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            var philippineNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone);

            var transaction = new SalesTransactionEntities
            {
                ReceiptNumber = $"HOLD{philippineNow.Year}{philippineNow:MMddHHmmss}",
                MenuItemName = menuItem.ItemName,
                DateTime = philippineNow,
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
                MenuItemSizeId = sizeId,
                Size = selectedSize,
                SugarLevel = buy.SugarLevel ?? "100%", // Default to 100% if not specified
                Quantity = buy.Quantity,
                UnitPrice = itemPrice
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
        public async Task<string> RequestVoidAsync(int saleId, string reason)
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
                Username = sale.CashierId,
                Role = "Staff",
                Action = "Request Void",
                Description = $"Requested void for sale #{sale.Id}. Reason: {reason}",
                Date = DateTime.Now
            });

            await _context.SaveChangesAsync();

            return $"Void request submitted for Sale #{sale.Id}. Awaiting admin approval.";
        }


        public async Task<string> ApproveVoidAsync(int saleId)
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
                Username = "Admin",
                Role = "Admin",
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


        public async Task<bool> CompleteGCashPurchaseAsync(string paymentReference, string cashierId)
        {
            try
            {
                Console.WriteLine($"Attempting to complete GCash purchase for reference: {paymentReference}");

                // Get pending payment data from PayMongo service
                var pendingPayment = _payMongo.GetType()
                    .GetMethod("GetPendingPayment")
                    ?.Invoke(_payMongo, new object[] { paymentReference }) as PendingPaymentDTO;

                if (pendingPayment == null)
                {
                    Console.WriteLine($"No pending payment found for {paymentReference}");
                    return false;
                }

                Console.WriteLine($"Found pending payment with {pendingPayment.CartItems.Count} items");
                Console.WriteLine($"Pending payment CashierId: {pendingPayment.CashierId}");
                Console.WriteLine($"Parameter cashierId: {cashierId}");

                // Get cashier - prioritize cashierId from pending payment, then parameter, then fallback to any cashier
                string actualCashierId = pendingPayment.CashierId ?? cashierId;
                
                if (string.IsNullOrEmpty(actualCashierId))
                {
                    // Get any cashier user for system-triggered completions as last resort
                    var anyCashier = await _context.UsersIdentity
                        .FirstOrDefaultAsync(u => u.UserName != null && u.UserName.Contains("cashier"));
                    
                    if (anyCashier != null)
                    {
                        actualCashierId = anyCashier.Id;
                        Console.WriteLine($"❌ Using fallback cashier {anyCashier.Email} (ID: {actualCashierId}) for webhook completion");
                    }
                    else
                    {
                        Console.WriteLine("❌ No cashier found in database");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine($"✅ Using cashier ID from pending payment: {actualCashierId}");
                }

                // Create the sales transaction
                var subtotal = 0m;
                var tax = 0m;
                var discount = 0m;

                // Build menu item names from cart items
                var menuItemNames = new List<string>();
                foreach (var cartItem in pendingPayment.CartItems)
                {
                    var menuItem = await _context.MenuItems.FindAsync(cartItem.MenuItemId);
                    if (menuItem != null)
                    {
                        menuItemNames.Add(menuItem.ItemName);
                    }
                }

                // Use Philippine Time for consistency with sales reports
                var philippineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
                var philippineNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, philippineTimeZone);

                Console.WriteLine($"🕐 GCash Transaction Philippine Time: {philippineNow:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"🕐 Philippine Date Only: {philippineNow.Date:yyyy-MM-dd}");

                var sale = new SalesTransactionEntities
                {
                    ReceiptNumber = $"REC{philippineNow.Year}{philippineNow:MMddHHmmss}",
                    MenuItemName = menuItemNames.Any() ? string.Join(", ", menuItemNames) : "GCash Purchase",
                    DateTime = philippineNow,
                    CashierId = actualCashierId,
                    BranchId = pendingPayment.BranchId,
                    Subtotal = pendingPayment.TotalAmount / (1 + pendingPayment.TaxPercent / 100m),
                    Tax = pendingPayment.TotalAmount - (pendingPayment.TotalAmount / (1 + pendingPayment.TaxPercent / 100m)),
                    Discount = (pendingPayment.TotalAmount / (1 + pendingPayment.TaxPercent / 100m)) * (pendingPayment.DiscountPercent / 100m),
                    Total = pendingPayment.TotalAmount,
                    PaymentMethod = "GCash",
                    Status = "Completed",
                    PaymentReference = paymentReference
                };

                _context.SalesTransaction.Add(sale);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Created sales transaction #{sale.Id} with CashierId: {sale.CashierId}, Status: {sale.Status}, PaymentMethod: {sale.PaymentMethod}");
                Console.WriteLine($"   📅 DateTime: {sale.DateTime}");
                Console.WriteLine($"   💵 Total: ₱{sale.Total:F2}");
                Console.WriteLine($"   🏢 BranchId: {sale.BranchId}");
                Console.WriteLine($"   📝 Receipt: {sale.ReceiptNumber}");
                Console.WriteLine($"   🔍 PaymentReference: {sale.PaymentReference}");
                
                // Verify the sale was actually saved with correct CashierId
                var verifyQuery = await _context.SalesTransaction
                    .Where(s => s.Id == sale.Id)
                    .Select(s => new { s.Id, s.CashierId, s.Status, s.PaymentMethod })
                    .FirstOrDefaultAsync();
                    
                if (verifyQuery != null)
                {
                    Console.WriteLine($"   ✅ VERIFIED in DB: SalesTransaction #{verifyQuery.Id} has CashierId={verifyQuery.CashierId}");
                }
                else
                {
                    Console.WriteLine($"   ❌ ERROR: Could not verify transaction #{sale.Id} in database!");
                }

                // Add sales items
                foreach (var cartItem in pendingPayment.CartItems)
                {
                    var menuItem = await _context.MenuItems
                        .Include(m => m.MenuItemProducts)
                            .ThenInclude(mp => mp.ProductOfSupplier)
                        .FirstOrDefaultAsync(m => m.Id == cartItem.MenuItemId);

                    if (menuItem == null)
                    {
                        Console.WriteLine($"Menu item {cartItem.MenuItemId} not found");
                        continue;
                    }

                    // Deduct stock
                    foreach (var menuItemProduct in menuItem.MenuItemProducts)
                    {
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.Id == menuItemProduct.ProductOfSupplierId);

                        if (product != null)
                        {
                            var totalToDeduct = menuItemProduct.QuantityUsed * cartItem.Quantity;
                            product.Stocks -= totalToDeduct;
                            _context.Products.Update(product);
                            Console.WriteLine($"Deducted {totalToDeduct} from product {product.ProductName}");
                        }
                    }

                    // Add sale item
                    var saleItem = new SalesItemEntities
                    {
                        SalesTransactionId = sale.Id,
                        MenuItemId = cartItem.MenuItemId,
                        MenuItemSizeId = cartItem.MenuItemSizeId,
                        Size = cartItem.Size ?? "Regular",
                        SugarLevel = cartItem.SugarLevel ?? "100%", // Default to 100% if not specified
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Price > 0 ? cartItem.Price : menuItem.Price
                    };

                    _context.SalesItems.Add(saleItem);
                    Console.WriteLine($"Added sale item: {menuItem.ItemName} x{cartItem.Quantity}");
                }

                // Add audit log
                _context.AuditLog.Add(new AuditLogEntities
                {
                    Username = actualCashierId,
                    Role = "Cashier",
                    Action = "GCash Payment Webhook",
                    Description = $"Completed GCash purchase via webhook. Reference: {paymentReference}, Total: ₱{pendingPayment.TotalAmount:F2}",
                    Date = DateTime.Now
                });

                await _context.SaveChangesAsync();

                Console.WriteLine($"Successfully completed GCash purchase for {paymentReference}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error completing GCash purchase: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<bool> IsGCashTransactionCompletedAsync(string paymentReference)
        {
            try
            {
                // Check if a completed transaction with this payment reference exists
                var existingTransaction = await _context.SalesTransaction
                    .FirstOrDefaultAsync(st => st.PaymentReference == paymentReference && st.Status == "Completed");

                return existingTransaction != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking transaction completion: {ex.Message}");
                return false;
            }
        }


    }


}
