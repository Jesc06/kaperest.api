using KapeRest.Application.Services.Cashiers.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Users.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverAllSalesController : ControllerBase
    {
        private readonly OverAllSalesService _overAllSalesService;
        public OverAllSalesController(OverAllSalesService overAllSalesService)
        {
            _overAllSalesService = overAllSalesService;
        }

        [HttpGet("AdminOverAllSales")]
        public async Task<ActionResult> GetAllSalesByAdmin()
        {
            var result = await _overAllSalesService.GetAllSalesByAdmin();
            return Ok(result);
        }
        [HttpGet("CashierOverAllSales")]
        public async Task<ActionResult> GetAllSalesByCashiers([FromQuery] string cashierId)
        {
            var result = await _overAllSalesService.GetAllSalesByCashiers(cashierId);
            return Ok(result);
        }

       


    }
}
