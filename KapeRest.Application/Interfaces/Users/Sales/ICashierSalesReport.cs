using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Core.Entities.SalesTransaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.Users.Sales
{
    public interface ICashierSalesReport
    {
        Task<ICollection<SalesReportDTO>> GetDailySalesReportByCashierAsync(string cashierId);
        Task<ICollection<SalesReportDTO>> GetWeeklySalesReportByCashierAsync(string cashierId);
        Task<ICollection<SalesReportDTO>> GetMonthlySalesReportByCashierAsync(string cashierId);
    }
}
