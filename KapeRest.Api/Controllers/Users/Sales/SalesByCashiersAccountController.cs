using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Services.Users.Sales;
using KapeRest.Application.UseCases.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Users.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesByCashiersAccountController : ControllerBase
    {
        private readonly SalesService _salesService;
        private readonly GenerateSalesReportUseCase _generateSalesReportUseCase;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SalesByCashiersAccountController(SalesService salesService, GenerateSalesReportUseCase generateSalesReportUseCase, IWebHostEnvironment webHostEnvironment)
        {
            _salesService = salesService;
            _generateSalesReportUseCase = generateSalesReportUseCase;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("GetSalesByCashiers")]
        public async Task<IActionResult> GetSalesByCashiers([FromBody]string salesCashierId)
        {
            var sales = await _salesService.GetSalesByCashiers(salesCashierId);
            return Ok(sales);
        }

        [HttpGet("CashierReports")]
        public async Task<ActionResult>GetSalesReport(string cashierId)
        {
            var logopath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "kapelogo.png");
           var result = await _generateSalesReportUseCase.ExecuteAsync(cashierId, logopath);
            return File(result, "application/pdf", "SalesReport.pdf");
        }


    }
}
