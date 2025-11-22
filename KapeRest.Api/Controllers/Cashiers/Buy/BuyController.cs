using KapeRest.Api.DTOs.Buy;
using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Services.Cashiers.Buy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace KapeRest.Controllers.Users.Buy
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyController : ControllerBase
    {
        private readonly BuyService _buyService;
        public BuyController(BuyService buyService)
        {
            _buyService = buyService;
        }

        [HttpPost("Buy")]
        public async Task<ActionResult> Buy(BuyDTO buy)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cashierIdFromJWTClaims = User.FindFirst("cashierId")?.Value;

            if (string.IsNullOrEmpty(cashierIdFromJWTClaims))
                return Unauthorized(new { error = "Cashier ID not found in token" });

            var buymenuItemDTO = new BuyMenuItemDTO
            {
                MenuItemId = buy.MenuItemId,
                Quantity = buy.Quantity,
                DiscountPercent = buy.DiscountPercent,
                Tax = buy.Tax,
                PaymentMethod = buy.PaymentMethod,
                CashierId = cashierIdFromJWTClaims
            };

            try
            {
                var result = await _buyService.BuyItem(buymenuItemDTO);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("HoldTransaction")]
        public async Task<ActionResult> HoldTransaction(BuyDTO buy)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cashierIdFromJWTClaims = User.FindFirst("cashierId")?.Value;

            if (string.IsNullOrEmpty(cashierIdFromJWTClaims))
                return Unauthorized(new { error = "Cashier ID not found in token" });

            var holdDTO = new BuyMenuItemDTO
            {
                MenuItemId = buy.MenuItemId,
                Quantity = buy.Quantity,
                DiscountPercent = buy.DiscountPercent,
                Tax = buy.Tax,
                PaymentMethod = buy.PaymentMethod,
                CashierId = cashierIdFromJWTClaims
            };

            try
            {
                var result = await _buyService.HoldTransaction(holdDTO);
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("UpdateHoldTransaction")]
        public async Task<ActionResult> UpdateHoldTransaction(UpdateHoldDTO update)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cashierIdFromJWTClaims = User.FindFirst("cashierId")?.Value;

            var updateHold = new UpdateHoldTransaction
            {
                SalesTransactionID = update.SalesTransactionID,
                Quantity = update.Quantity,
                DiscountPercent = update.DiscountPercent,
                Tax = update.Tax,
                PaymentMethod = update.PaymentMethod,
                CashierId = cashierIdFromJWTClaims!
            };

            var result = await _buyService.UpdateHeldTransaction(updateHold);
            return Ok(result);
        }

        [HttpPost("ResumeTransaction")]
        public async Task<ActionResult> ResumeTransaction([FromQuery] int saleId)
        {
            var result = await _buyService.ResumeHoldAsync(saleId);
            return Ok(result);
        } 

        [HttpPost("VoidItem")]
        public async Task<ActionResult> VoidItem([FromQuery] int saleItemId)
        {
            var result = await _buyService.VoidItemAsync(saleItemId); 
            return Ok(result);
        }

        [HttpPost("CancelHold{saleId}")]
        public async Task<ActionResult> CancelHold(int saleId)
        {
            var result = await _buyService.CancelHoldAsync(saleId); 
            return Ok(result);
        }


        [HttpGet("GetHoldTransactions")]
        public async Task<ActionResult> GetHoldTransactions(string cashierId)
        {
            if (string.IsNullOrEmpty(cashierId))
                return BadRequest("Cashier ID is required");

            try
            {
                var result = await _buyService.GetHoldTransactions(cashierId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


    }
}
