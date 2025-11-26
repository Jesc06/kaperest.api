using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Cashiers.Sales
{
    public class AdminSalesReportsRepository : IAdminSalesReport
    {
        private readonly ApplicationDbContext _context;

        public AdminSalesReportsRepository(ApplicationDbContext context)
        {
            _context = context;
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

        private async Task<ICollection<SalesReportDTO>> GetSalesReportInRangeAsync(DateTime startUtc, DateTime endUtc)
        {
            var data = await _context.SalesTransaction
                .Where(s => s.DateTime >= startUtc
                            && s.DateTime < endUtc
                            && s.Status == "Completed")
                .Select(s => new SalesReportDTO
                {
                    Id = s.Id,
                    Username = s.CashierId ?? "Unknown",
                    FullName = "N/A",
                    Email = "N/A",
                    BranchName = "N/A",
                    BranchLocation = "N/A",
                    MenuItemName = s.MenuItemName,
                    DateTime = s.DateTime,
                    Subtotal = s.Subtotal,
                    Tax = s.Tax,
                    Discount = s.Discount,
                    Total = s.Total,
                    Status = s.Status
                })
                .ToListAsync();

            return data;
        }

        #endregion

        #region -- Sales Reports (Based on Philippine Local Time) --

        public async Task<ICollection<SalesReportDTO>> GetDailySalesReportAsync()
        {
            var phNow = GetPhilippineNow();
            var startOfDay = new DateTime(phNow.Year, phNow.Month, phNow.Day, 0, 0, 0);
            var endOfDay = startOfDay.AddDays(1);
            var (startUtc, endUtc) = GetUtcRange(startOfDay, endOfDay);

            return await GetSalesReportInRangeAsync(startUtc, endUtc);
        }

        public async Task<ICollection<SalesReportDTO>> GetMonthlySalesReportAsync()
        {
            var phNow = GetPhilippineNow(); 
            var startOfMonth = new DateTime(phNow.Year, phNow.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            var (startUtc, endUtc) = GetUtcRange(startOfMonth, endOfMonth);

            return await GetSalesReportInRangeAsync(startUtc, endUtc);
        }

        public async Task<ICollection<SalesReportDTO>> GetYearlySalesReportAsync()
        {
            var phNow = GetPhilippineNow();
            var startOfYear = new DateTime(phNow.Year, 1, 1, 0, 0, 0);
            var endOfYear = startOfYear.AddYears(1);
            var (startUtc, endUtc) = GetUtcRange(startOfYear, endOfYear);

            return await GetSalesReportInRangeAsync(startUtc, endUtc);
        }

        #endregion
    }
}
