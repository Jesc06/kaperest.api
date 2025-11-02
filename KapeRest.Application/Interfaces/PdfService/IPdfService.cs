using KapeRest.Application.DTOs.Users.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.Interfaces.PdfService
{
    public interface IPdfService
    {
        byte[] GenerateSalesReport(IEnumerable<SalesReportDTO> sales, string logopath);
    }
}
