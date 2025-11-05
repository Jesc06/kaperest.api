using KapeRest.Application.DTOs.PayMongo;
using KapeRest.Application.Interfaces.PayMongo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Cashiers.PayMongo
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayGcashController : ControllerBase
    {
        private readonly IPayMongo _pay;
        public PayGcashController(IPayMongo pay)
        {
            _pay = pay;
        }

        [HttpPost("PayGcash")]
        public async Task<ActionResult> PayWithGcash([FromBody]CreatePaymentDTO dto)
        {
            var result = await _pay.CreateGcashPaymentAsync(dto);   
            return Ok(result);  
        }

        [HttpGet("GenerateGcashQrCode")]
        public ActionResult GenerateGcashQrCode([FromQuery] string checkoutUrl)
        {
            var qrCodeBytes = _pay.GenerateQrCode(checkoutUrl);
            return File(qrCodeBytes, "image/png");
        }


    }
}
