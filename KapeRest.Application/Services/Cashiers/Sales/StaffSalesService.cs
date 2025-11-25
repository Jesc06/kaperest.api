using KapeRest.Application.DTOs.Cashiers;
using KapeRest.Application.Interfaces.Cashiers.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Services.Cashiers.Sales
{
    public class StaffSalesService
    {
        private readonly IStaffSalesReport _salesRepo;
        public StaffSalesService(IStaffSalesReport salesRepo)
        {
            _salesRepo = salesRepo;
        }

        public async Task<ICollection<StaffSalesReportDTO>> GetDailySales()
        {
            var result = await _salesRepo.GetDailySalesReportAsync();
            return result;
        }

        public async Task<ICollection<StaffSalesReportDTO>> GetMonthlySales()
        {
            var result = await _salesRepo.GetMonthlySalesReportAsync();
            return result;
        }

        public async Task<ICollection<StaffSalesReportDTO>> GetWeeklySales()
        {
            var result = await _salesRepo.GetWeeklySalesReportAsync();
            return result;
        }

        public async Task<ICollection<StaffSalesReportDTO>> GetCustomSales(DateTime startDate, DateTime endDate)
        {
            var result = await _salesRepo.GetCustomSalesReportAsync(startDate, endDate);
            return result;
        }   



    }
}
