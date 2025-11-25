using KapeRest.Application.Services.Cashiers.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Cashiers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffSalesReportController : ControllerBase
    {

        private readonly StaffSalesService _staffSalesService;
        public StaffSalesReportController(StaffSalesService staffSalesService)
        {
            _staffSalesService = staffSalesService;
        }
        [HttpGet("Daily")]
        public async Task<ActionResult> GetDailySalesReport()
        {
            var result = await _staffSalesService.GetDailySales();
            return Ok(result);
        }
        [HttpGet("Weekly")]
        public async Task<ActionResult> GetWeeklySalesReport()
        {
            var result = await _staffSalesService.GetWeeklySales();
            return Ok(result);
        }
        [HttpGet("Monthly")]
        public async Task<ActionResult> GetMonthlySalesReport()
        {
            var result = await _staffSalesService.GetMonthlySales();
            return Ok(result);
        }
        [HttpGet("Custom")]
        public async Task<ActionResult> GetCustomSalesReport(DateTime startDate, DateTime endDate)
        {
            var result = await _staffSalesService.GetCustomSales(startDate, endDate);
            return Ok(result);
        }


    }
}
