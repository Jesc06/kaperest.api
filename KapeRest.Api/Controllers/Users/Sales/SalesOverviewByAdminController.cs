using KapeRest.Application.Services.Users.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Users.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOverviewByAdminController : ControllerBase
    {
        private readonly SalesService _salesService;
        public SalesOverviewByAdminController(SalesService salesService)
        {
            _salesService = salesService;
        }

        [HttpPost("GetSalesByAdmin")]
        public async Task<IActionResult> GetSalesByAdmin()
        {
            var sales = await _salesService.GetSalesByAdmin();
            return Ok(sales);
        }

    }
}
