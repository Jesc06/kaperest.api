using KapeRest.Application.Interfaces.Cashiers.Sales;
using KapeRest.Application.Interfaces.PdfService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KapeRest.Application.UseCases.Sales
{
    public class GenerateSalesReportUseCase
    {
        private readonly IAdminSalesReport _AdminsalesReport;
        private readonly ICashierSalesReport _cashierSalesReport;
        private readonly IPdfService _pdfService;
        public GenerateSalesReportUseCase(IAdminSalesReport AdminsalesReport,ICashierSalesReport cashierSalesReport, IPdfService pdfService)
        {
            _AdminsalesReport = AdminsalesReport;
            _cashierSalesReport = cashierSalesReport;
            _pdfService = pdfService;
        }
        #region--AdminSalesReport-- 
        public async Task<byte[]> AdminDailySalesReport(string logopath)
        {
            var sales = await _AdminsalesReport.GetDailySalesReportAsync();
            return _pdfService.GenerateSalesReport(sales, logopath, "Admin");
        }
        public async Task<byte[]> AdminYearlySalesReport(string logopath)
        {
            var sales = await _AdminsalesReport.GetYearlySalesReportAsync();
            return _pdfService.GenerateSalesReport(sales, logopath, "Admin");
        }
        public async Task<byte[]> AdminMonthlySalesReport(string logopath)
        {
            var sales = await _AdminsalesReport.GetMonthlySalesReportAsync();
            return _pdfService.GenerateSalesReport(sales, logopath, "Admin");
        }
        #endregion

        #region--CashierSalesReport--
        public async Task<byte[]> CashierDailySalesReport(string cashierId, string logopath)
        {
            var sales = await _cashierSalesReport.GetDailySalesReportByCashierAsync(cashierId);
            return _pdfService.GenerateSalesReport(sales, logopath, "Cashier");
        }
        public async Task<byte[]> CashierYearlySalesReport(string cashierId, string logopath)
        {
            var sales = await _cashierSalesReport.GetYearlySalesReportByCashierAsync(cashierId);
            return _pdfService.GenerateSalesReport(sales, logopath, "Cashier");
        }
        public async Task<byte[]> CashierMonthlySalesReport(string cashierId, string logopath)
        {
            var sales = await _cashierSalesReport.GetMonthlySalesReportByCashierAsync(cashierId);
            return _pdfService.GenerateSalesReport(sales, logopath, "Cashier");
        }
        #endregion

    }
}
