using KapeRest.Api.DTOs.Buy;
using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Interfaces.Cashiers.Buy;
using KapeRest.Application.Services.Cashiers.Buy;
using KapeRest.Infrastructures.Persistence.Repositories.Cashiers.Buy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
                CashierId = cashierIdFromJWTClaims,
                VoucherCode = buy.VoucherCode,
                CustomerName = buy.CustomerName,
                CustomerId = buy.CustomerId
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

        [HttpPost("ResumeTransaction")]
        public async Task<ActionResult> ResumeTransaction([FromQuery] int saleId)
        {
            var result = await _buyService.ResumeHoldAsync(saleId);
            return Ok(result);
        } 

        [HttpPost("VoidItem")]
        public async Task<ActionResult> VoidItem([FromQuery] int saleItemId)
        {
            var userIdFromJwt = User.FindFirst("uid")?.Value;
            var roleFromJwt = User.FindFirst("role")?.Value ?? "Cashier";

            var result = await _buyService.VoidItemAsync(saleItemId, userIdFromJwt, roleFromJwt); 
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


        //Void Request from cashier

        [HttpPost("RequestVoid")]
        public async Task<ActionResult> RequestVoid(int saleId, string reason)
        {
            var result = await _buyService.RequestVoidAsync(saleId, reason);
            return Ok(result);
        }
        [HttpPost("ApprovedVoid")]
        public async Task<ActionResult> ApprovedVoid(int saleId)
        {   
            var result = await _buyService.ApproveVoidAsync(saleId);
            return Ok(result);
        }
        [HttpPost("RejectVoid")]
        public async Task<ActionResult> RejectVoid(int saleId)
        {
            var userIdFromJwt = User.FindFirst("uid")?.Value;
            var roleFromJwt = User.FindFirst("role")?.Value ?? "Admin";

            var result = await _buyService.RejectVoidAsync(saleId, userIdFromJwt, roleFromJwt);
            return Ok(result);
        }





    }
}
