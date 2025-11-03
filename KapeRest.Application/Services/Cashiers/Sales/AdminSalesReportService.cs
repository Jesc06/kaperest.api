using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Cashiers.Sales
{
    public class AdminSalesReportService
    {
        private readonly IAdminSalesReport _salesReport;
        public AdminSalesReportService(IAdminSalesReport salesReport)
        {
            _salesReport = salesReport;
        }

        public async Task<ICollection<SalesReportDTO>> GetDailySaleReport()
        {
            return await _salesReport.GetDailySalesReportAsync();
        }
        public async Task<ICollection<SalesReportDTO>> GetWeeklySaleReport()
        {
            return await _salesReport.GetWeeklySalesReportAsync();
        }
        public async Task<ICollection<SalesReportDTO>> GetMonthlySaleReport()
        {
            return await _salesReport.GetMonthlySalesReportAsync();
        }


    }
}
