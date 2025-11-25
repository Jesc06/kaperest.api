using KapeRest.Application.Services.Cashiers.Sales;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Cashiers.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {

        private readonly SalesTransactionService _salesPurchases;
        public PurchasesController(SalesTransactionService salesPurchases)
        {
            _salesPurchases = salesPurchases;
        }

        [HttpGet("SalesPurchases")]
        public async Task<IActionResult> GetSalesPurchases()
        {
            var sales = await _salesPurchases.SalesPurchases();
            return Ok(sales);
        }

    }
}
