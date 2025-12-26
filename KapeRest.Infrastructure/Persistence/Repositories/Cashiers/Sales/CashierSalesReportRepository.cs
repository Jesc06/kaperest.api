using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Core.Entities.SalesTransaction;
using KapeRest.Infrastructure.Persistence.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KapeRest.Infrastructure.Persistence.Database;

namespace KapeRest.Infrastructure.Persistence.Repositories.Cashiers.Sales
{
    public class CashierSalesReportRepository : ICashierSalesReport
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<UsersIdentity> _userManager;

        public CashierSalesReportRepository(ApplicationDbContext context, UserManager<UsersIdentity> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        #region -- Helper Methods --
        private static DateTime GetPhilippineNow() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila"));

        private (DateTime startUtc, DateTime endUtc) GetUtcRange(DateTime start, DateTime end)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila");
            return (
                TimeZoneInfo.ConvertTimeToUtc(start, tz),
                TimeZoneInfo.ConvertTimeToUtc(end, tz)
            );
        }

        private async Task<ICollection<SalesReportDTO>> GetSalesReportByCashierInRangeAsync(
            string cashierId,
            DateTime start,
            DateTime end)
        {
            Console.WriteLine($"🔍 Querying sales for cashier {cashierId}");
            Console.WriteLine($"🔍 Date range: {start:yyyy-MM-dd HH:mm:ss} to {end:yyyy-MM-dd HH:mm:ss}");
            
            var allTransactions = await _context.SalesTransaction
                .Where(s => s.CashierId == cashierId && s.Status == "Completed")
                .ToListAsync();
            
            Console.WriteLine($"🔍 Total completed transactions for cashier: {allTransactions.Count}");
            foreach (var t in allTransactions)
            {
                Console.WriteLine($"   - {t.DateTime:yyyy-MM-dd HH:mm:ss} | {t.MenuItemName} | ₱{t.Total} | Match: {(t.DateTime >= start && t.DateTime < end)}");
            }
            
            var data = await (from s in _context.SalesTransaction
                              join u in _context.UsersIdentity on s.CashierId equals u.Id
                              join b in _context.Branches on u.BranchId equals b.Id into branchJoin
                              from bj in branchJoin.DefaultIfEmpty()
                              where s.CashierId == cashierId &&
                                    s.DateTime >= start &&
                                    s.DateTime < end &&
                                    s.Status == "Completed"
                              select new SalesReportDTO
                              {
                                  Id = s.Id,
                                  Username = u.UserName,
                                  FullName = $"{u.FirstName} {u.MiddleName} {u.LastName}",
                                  Email = u.Email,
                                  BranchName = bj != null ? bj.BranchName : "N/A",
                                  BranchLocation = bj != null ? bj.Location : "N/A",
                                  MenuItemName = s.MenuItemName,
                                  DateTime = s.DateTime,
                                  Subtotal = s.Subtotal,
                                  Tax = s.Tax,
                                  Discount = s.Discount,
                                  Total = s.Total,
                                  Status = s.Status
                              }).ToListAsync();
            return data;
        }
        #endregion

        
        #region -- Sales Reports Per Cashier (Philippine Time) --
        public async Task<ICollection<SalesReportDTO>> GetDailySalesReportByCashierAsync(string cashierId)
        {
            var phNow = GetPhilippineNow();
            var startOfDay = new DateTime(phNow.Year, phNow.Month, phNow.Day, 0, 0, 0);
            var endOfDay = startOfDay.AddDays(1);
            
            Console.WriteLine($"Daily Sales Query for Cashier: {cashierId}");
            Console.WriteLine($"Philippine Now: {phNow:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Start of Day: {startOfDay:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"End of Day: {endOfDay:yyyy-MM-dd HH:mm:ss}");
            
            // Since we're now storing DateTime as Philippine Time directly,
            // we don't need to convert to UTC for the query
            var result = await GetSalesReportByCashierInRangeAsync(cashierId, startOfDay, endOfDay);
            
            Console.WriteLine($"Daily Sales Found: {result.Count} transactions");
            foreach (var sale in result)
            {
                Console.WriteLine($"   - {sale.DateTime:yyyy-MM-dd HH:mm:ss} | {sale.MenuItemName} | ₱{sale.Total}");
            }
            return result;
        }

        //WEEKLY → YEARLY
        public async Task<ICollection<SalesReportDTO>> GetYearlySalesReportByCashierAsync(string cashierId)
        {
            var phNow = GetPhilippineNow();
            var startOfYear = new DateTime(phNow.Year, 1, 1);
            var endOfYear = startOfYear.AddYears(1);
            // Since we're now storing DateTime as Philippine Time directly,
            // we don't need to convert to UTC for the query
            return await GetSalesReportByCashierInRangeAsync(cashierId, startOfYear, endOfYear);
        }

        public async Task<ICollection<SalesReportDTO>> GetMonthlySalesReportByCashierAsync(string cashierId)
        {
            var phNow = GetPhilippineNow();
            var startOfMonth = new DateTime(phNow.Year, phNow.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            // Since we're now storing DateTime as Philippine Time directly,
            // we don't need to convert to UTC for the query
            return await GetSalesReportByCashierInRangeAsync(cashierId, startOfMonth, endOfMonth);
        }
        #endregion
        
    }
}
