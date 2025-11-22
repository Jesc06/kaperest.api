using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Infrastructures.Persistence.Database;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<UsersIdentity> _userManager;

        public AdminSalesReportsRepository(ApplicationDbContext context, UserManager<UsersIdentity> userManager)
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

        private async Task<ICollection<SalesReportDTO>> GetSalesReportInRangeAsync(DateTime startUtc, DateTime endUtc)
        {
            var data = await (from s in _context.SalesTransaction
                              join u in _context.UsersIdentity on s.CashierId equals u.Id
                              join b in _context.Branches on u.BranchId equals b.Id into branchJoin
                              from bj in branchJoin.DefaultIfEmpty()
                              where s.DateTime >= startUtc
                                    && s.DateTime < endUtc
                                    && s.Status == "Completed" 
                              select new SalesReportDTO
                              {
                                  Id = s.Id,
                                  Username = u.UserName,
                                  FullName = $"{u.FirstName}\t{u.MiddleName}\t{u.LastName}",
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

        #region -- Sales Reports (Based on Philippine Local Time) --

        public async Task<ICollection<SalesReportDTO>> GetDailySalesReportAsync()
        {
            var phNow = GetPhilippineNow();
            var startOfDay = new DateTime(phNow.Year, phNow.Month, phNow.Day, 0, 0, 0);
            var endOfDay = startOfDay.AddDays(1);
            var (startUtc, endUtc) = GetUtcRange(startOfDay, endOfDay);

            return await GetSalesReportInRangeAsync(startUtc, endUtc);
        }

        public async Task<ICollection<SalesReportDTO>> GetWeeklySalesReportAsync()
        {
            var phNow = GetPhilippineNow();
            int diff = (7 + (phNow.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = phNow.AddDays(-diff).Date;
            var endOfWeek = startOfWeek.AddDays(7);
            var (startUtc, endUtc) = GetUtcRange(startOfWeek, endOfWeek);

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
        #endregion

    }
}
