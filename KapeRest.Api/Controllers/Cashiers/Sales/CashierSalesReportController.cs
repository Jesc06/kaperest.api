using System.IO;
using System.Threading.Tasks;
using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Services.Cashiers.Sales;
using KapeRest.Application.UseCases.Sales;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Users.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashierSalesReportController : ControllerBase
    {
        private readonly CashierSalesReportService _salesService;
        private readonly GenerateSalesReportUseCase _generateSalesReportUseCase;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CashierSalesReportController(CashierSalesReportService salesService, GenerateSalesReportUseCase generateSalesReportUseCase, IWebHostEnvironment webHostEnvironment)
        {
            _salesService = salesService;
            _generateSalesReportUseCase = generateSalesReportUseCase;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("CashierDailySales")]
        public async Task<ActionResult> GetCashierDailySalesReport(string cashierId)
        {
            var result = await _salesService.DailyReport(cashierId);
            return Ok(result);
        }
        [HttpGet("CashierYearlySales")]
        public async Task<ActionResult> GetCashierYearlySalesReport(string cashierId)
        {
            var result = await _salesService.GetYearlySalesReportByCashierAsync(cashierId);
            return Ok(result);
        }
        [HttpGet("CashierMonthlySales")]
        public async Task<ActionResult> GetCashierMonthlySalesReport(string cashierId)
        {
            var result = await _salesService.MonthlyReport(cashierId);
            return Ok(result);
        }


        #region--Generate PDF Report--
        [HttpGet("CashierGenerateDailyPdfReports")]
        public async Task<ActionResult> CashierGenerateDailyPdfReports(string cashierId)
        {
            var logopath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "kapelogo.png");
            var result = await _generateSalesReportUseCase.CashierDailySalesReport(cashierId, logopath);
            return File(result, "application/pdf", "CashierDailySalesReport.pdf");
        }
        [HttpGet("CashierGenerateYearlyPdfReports")]
        public async Task<ActionResult> CashierGenerateWeeklyPdfReports(string cashierId)
        {
            var logopath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "kapelogo.png");
            var result = await _generateSalesReportUseCase.CashierYearlySalesReport(cashierId, logopath);
            return File(result, "application/pdf", "CashierWeeklySalesReport.pdf");
        }
        [HttpGet("CashierGenerateMonthlyPdfReports")]
        public async Task<ActionResult> CashierGenerateMonthlyPdfReports(string cashierId)
        {
            var logopath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "kapelogo.png");
            var result = await _generateSalesReportUseCase.CashierMonthlySalesReport(cashierId, logopath);
            return File(result, "application/pdf", "CashierMonthlySalesReport.pdf");
        }
        #endregion

    }
}
