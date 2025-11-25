using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.DTOs.Cashiers
{
    public class StaffSalesReportDTO
    {
        public string Date { get; set; } // e.g., "2025-11-23"
        public decimal TotalSales { get; set; }
        public int TransactionCount { get; set; }
    }
}
