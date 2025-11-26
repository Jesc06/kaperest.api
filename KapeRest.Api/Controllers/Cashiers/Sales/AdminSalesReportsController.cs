using KapeRest.Application.Services.Cashiers.Sales;
using KapeRest.Application.UseCases.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Users.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSalesReportsController : ControllerBase
    {
        private readonly AdminSalesReportService _salesReportService;
        private readonly GenerateSalesReportUseCase _generateSalesReportUseCase;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminSalesReportsController(AdminSalesReportService salesReportService, 
                                           GenerateSalesReportUseCase generateSalesReportUseCase, 
                                           IWebHostEnvironment webHostEnvironment)
        {
            _salesReportService = salesReportService;
            _generateSalesReportUseCase = generateSalesReportUseCase;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("AdminDailyReports")]
        public async Task<ActionResult> DailyReports()
        {
            var result = await _salesReportService.GetDailySaleReport();
            return Ok(result);
        }
        [HttpGet("AdminYearlyReports")]
        public async Task<ActionResult> YearlyReports()
        {
            var result = await _salesReportService.GetYearlySaleReport();
            return Ok(result);
        }
        [HttpGet("AdminMonthlyReports")]
        public async Task<ActionResult> MonthlyReports()
        {
            var result = await _salesReportService.GetMonthlySaleReport();
            return Ok(result);
        }

        #region--Generate PDF Report--
        [HttpGet("AdminGenerateDailyPdfReports")]
        public async Task<ActionResult> AdminGenerateDailyPdfReports()
        {
            var logopath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "kapelogo.png");
            var result = await _generateSalesReportUseCase.AdminDailySalesReport(logopath);
            return File(result, "application/pdf", "AdminDailySalesReport.pdf");
        }
        [HttpGet("AdminGenerateYearlyPdfReports")]
        public async Task<ActionResult> AdminGenerateYearlyPdfReports()
        {
            var logopath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "kapelogo.png");
            var result = await _generateSalesReportUseCase.AdminYearlySalesReport(logopath);
            return File(result, "application/pdf", "AdminWeeklySalesReport.pdf");
        }
        [HttpGet("AdminGenerateMonthlyPdfReports")]
        public async Task<ActionResult> AdminGenerateMonthlyPdfReports()
        {
            var logopath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "kapelogo.png");
            var result = await _generateSalesReportUseCase.AdminMonthlySalesReport(logopath);
            return File(result, "application/pdf", "AdminMonthlySalesReport.pdf");
        }
        #endregion


    }
}
