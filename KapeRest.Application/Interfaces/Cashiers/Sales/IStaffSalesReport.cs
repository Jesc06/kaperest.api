using KapeRest.Application.DTOs.Cashiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Cashiers.Sales
{
    public interface IStaffSalesReport
    {
        Task<ICollection<StaffSalesReportDTO>> GetDailySalesReportAsync();
        Task<ICollection<StaffSalesReportDTO>> GetWeeklySalesReportAsync();
        Task<ICollection<StaffSalesReportDTO>> GetMonthlySalesReportAsync();
        Task<ICollection<StaffSalesReportDTO>> GetCustomSalesReportAsync(DateTime startDate, DateTime endDate);
    }
}
