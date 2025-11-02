using KapeRest.Application.DTOs.Users.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Users.Sales
{
    public interface IgenerateAdminPdfReports
    {
        Task<ICollection<SalesReportDTO>> GetDailySalesReportAsync();
        Task<ICollection<SalesReportDTO>> GetWeeklySalesReportAsync();
        Task<ICollection<SalesReportDTO>> GetMonthlySalesReportAsync();
    }
}
