using KapeRest.Application.DTOs.Users.Buy;
using KapeRest.Application.Services.Users.Buy;
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
        public async Task<ActionResult> Buy(BuyMenuItemDTO buy)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _buyService.BuyItem(buy);
            return Ok(result);
        }

        [HttpPost("HoldTransaction")]
        public async Task<ActionResult> HoldTransaction(BuyMenuItemDTO buy)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = await _buyService.HoldTransaction(buy);
            return Ok(result);
        }

        [HttpPost("ResumeTransaction")]
        public async Task<ActionResult> ResumeTransaction([FromQuery] int saleId)
        {
            var result = await _buyService.ResumeHoldAsync(saleId);
            return Ok(result);
        }

        [HttpPost("CancelHold{saleId}")]
        public async Task<ActionResult> CancelHold([FromQuery] int saleId)
        {
            var result = await _buyService.CancelHoldAsync(saleId); 
            return Ok(result);
        }

    }
}
