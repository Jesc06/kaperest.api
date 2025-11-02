using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Services.Users.Sales;
using KapeRest.Application.UseCases.Sales;
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
        [HttpGet("CashierWeeklySales")]
        public async Task<ActionResult> GetCashierWeeklySalesReport(string cashierId)
        {
            var result = await _salesService.WeeklyReport(cashierId);
            return Ok(result);
        }
        [HttpGet("CashierMonthlySales")]
        public async Task<ActionResult> GetCashierMonthlySalesReport(string cashierId)
        {
            var result = await _salesService.MonthlyReport(cashierId);
            return Ok(result);
        }


        /*
        [HttpGet("CashierReports")]
        public async Task<ActionResult>GetSalesReport(string cashierId)
        {
            var logopath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "kapelogo.png");
           var result = await _generateSalesReportUseCase.ExecuteAsync(cashierId, logopath);
            return File(result, "application/pdf", "SalesReport.pdf");
        }
        */

    }
}
