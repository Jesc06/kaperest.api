using KapeRest.Application.DTOs.Users.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Cashiers.Sales
{
    public interface IAdminSalesReport
    {
        Task<ICollection<SalesReportDTO>> GetDailySalesReportAsync();
        Task<ICollection<SalesReportDTO>> GetYearlySalesReportAsync();
        Task<ICollection<SalesReportDTO>> GetMonthlySalesReportAsync();
    }
}
