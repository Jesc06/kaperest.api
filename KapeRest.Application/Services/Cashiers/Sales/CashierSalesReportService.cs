using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Core.Entities.SalesTransaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Cashiers.Sales
{
    public class CashierSalesReportService
    {
        private readonly ICashierSalesReport _salesRepo;
        public CashierSalesReportService(ICashierSalesReport salesRepo)
        {
            _salesRepo = salesRepo;
        }

        public async Task<ICollection<SalesReportDTO>> DailyReport(string cashierId)
        {
            return await _salesRepo.GetDailySalesReportByCashierAsync(cashierId);
        }
        public async Task<ICollection<SalesReportDTO>> GetYearlySalesReportByCashierAsync(string cashierId)
        {
            return await _salesRepo.GetYearlySalesReportByCashierAsync(cashierId);
        }
        public async Task<ICollection<SalesReportDTO>> MonthlyReport(string cashierId)
        {
            return await _salesRepo.GetMonthlySalesReportByCashierAsync(cashierId);
        }

    }
}
