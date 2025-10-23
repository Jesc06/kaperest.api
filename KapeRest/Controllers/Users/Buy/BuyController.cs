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
        public async Task<ActionResult> Buy(int menuId)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _buyService.BuyItem(menuId);
            return Ok(result);
        }

    }
}
