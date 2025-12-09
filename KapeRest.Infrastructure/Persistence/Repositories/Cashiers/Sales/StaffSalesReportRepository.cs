using KapeRest.Application.DTOs.Cashiers;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KapeRest.Infrastructure.Persistence.Repositories.Cashiers.Sales
{
    public class StaffSalesReportRepository : IStaffSalesReport
    {
        private readonly ApplicationDbContext _context;

        public StaffSalesReportRepository(ApplicationDbContext context)
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

        //Aggregated sales data: Group by date and sum totals
        private async Task<ICollection<StaffSalesReportDTO>> GetAggregatedSalesInRangeAsync(
            DateTime startUtc, DateTime endUtc)
        {
            var data = await (from s in _context.SalesTransaction
                              where s.DateTime >= startUtc &&
                                    s.DateTime < endUtc &&
                                    s.Status == "Completed"
                              group s by s.DateTime.Date into g // Group by date (Philippine time)
                              select new StaffSalesReportDTO
                              {
                                  Date = g.Key.ToString("yyyy-MM-dd"), // Date in YYYY-MM-DD
                                  TotalSales = g.Sum(x => x.Total),
                                  TransactionCount = g.Count()
                              }).ToListAsync();

            return data.OrderBy(d => d.Date).ToList(); // Sort by date
        }
        #endregion

        #region -- Aggregated Sales Reports (Philippine Time) --
        public async Task<ICollection<StaffSalesReportDTO>> GetDailySalesReportAsync()
        {
            var phNow = GetPhilippineNow();
            var startOfDay = new DateTime(phNow.Year, phNow.Month, phNow.Day, 0, 0, 0);
            var endOfDay = startOfDay.AddDays(1);
            var (startUtc, endUtc) = GetUtcRange(startOfDay, endOfDay);
            return await GetAggregatedSalesInRangeAsync(startUtc, endUtc);
        }

        public async Task<ICollection<StaffSalesReportDTO>> GetWeeklySalesReportAsync()
        {
            var phNow = GetPhilippineNow();
            var startOfWeek = phNow.AddDays(-(int)phNow.DayOfWeek);
            var endOfWeek = startOfWeek.AddDays(7);
            var (startUtc, endUtc) = GetUtcRange(startOfWeek, endOfWeek);
            return await GetAggregatedSalesInRangeAsync(startUtc, endUtc);
        }

        public async Task<ICollection<StaffSalesReportDTO>> GetMonthlySalesReportAsync()
        {
            var phNow = GetPhilippineNow();
            var startOfMonth = new DateTime(phNow.Year, phNow.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1);
            var (startUtc, endUtc) = GetUtcRange(startOfMonth, endOfMonth);
            return await GetAggregatedSalesInRangeAsync(startUtc, endUtc);
        }
        public async Task<ICollection<StaffSalesReportDTO>> GetCustomSalesReportAsync(DateTime startDate, DateTime endDate)
        {
            var (startUtc, endUtc) = GetUtcRange(startDate, endDate);
            return await GetAggregatedSalesInRangeAsync(startUtc, endUtc);
        }
        #endregion










            



    }
}