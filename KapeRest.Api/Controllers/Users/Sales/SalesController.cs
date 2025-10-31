using KapeRest.Application.DTOs.Users.Sales;
using KapeRest.Application.Services.Users.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Users.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly SalesService _salesService;
        public SalesController(SalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpPost("GetSalesByCashiers")]
        public async Task<IActionResult> GetSalesByCashiers([FromBody]SalesDTO salesCashierId)
        {
            var sales = await _salesService.GetSalesByCashiers(salesCashierId);
            return Ok(sales);
        }

    }
}
