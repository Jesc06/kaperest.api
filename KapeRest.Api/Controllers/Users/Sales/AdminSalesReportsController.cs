using KapeRest.Application.Services.Users.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Users.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminSalesReportsController : ControllerBase
    {
        private readonly AdminSalesReportService _salesReportService;
        public AdminSalesReportsController(AdminSalesReportService salesReportService)
        {
            _salesReportService = salesReportService;
        }

        [HttpGet("AdminDailyReports")]
        public async Task<ActionResult> DailyReports()
        {
            var result = await _salesReportService.GetDailySaleReport();
            return Ok(result);
        }

        [HttpGet("AdminWeeklyReports")]
        public async Task<ActionResult> WeeklyReports()
        {
            var result = await _salesReportService.GetWeeklySaleReport();
            return Ok(result);
        }

        [HttpGet("AdminMonthlyReports")]
        public async Task<ActionResult> MonthlyReports()
        {
            var result = await _salesReportService.GetMonthlySaleReport();
            return Ok(result);
        }


    }
}
