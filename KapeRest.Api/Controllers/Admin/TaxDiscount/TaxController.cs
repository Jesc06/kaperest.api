using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Services.Admin.TaxDiscount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Admin.TaxDiscount
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController : ControllerBase
    {
        private readonly TaxDiscountService _taxDiscountService;
        public TaxController(TaxDiscountService taxDiscountService)
        {
            _taxDiscountService = taxDiscountService;
        }

        [HttpPost("AddTax")]
        public async Task<ActionResult> AddTax(TaxDiscountDTO dto)
        {
            var userIdFromJwt = User.FindFirst("sub")?.Value;
            var roleFromJwt = User.FindFirst("role")?.Value ?? "Admin";

            var add = await _taxDiscountService.AddTax(dto, userIdFromJwt, roleFromJwt);
            return Ok(add);
        }

        [HttpPut("UpdateTax")]
        public async Task<ActionResult> UpdateDiscount(UpdateTaxDiscountDTO dto)
        {
            var userIdFromJwt = User.FindFirst("sub")?.Value;
            var roleFromJwt = User.FindFirst("role")?.Value ?? "Admin";

            var update = await _taxDiscountService.UpdateDiscount(dto, userIdFromJwt, roleFromJwt);
            return Ok(update);
        }

        [HttpGet("GetAllTax")]
        public async Task<ActionResult> GetAllDiscount()
        {
            var getAll = await _taxDiscountService.GetAllDiscount();
            return Ok(getAll);
        }

        [HttpDelete("DeleteTax")]
        public async Task<ActionResult> DeleteTaxAndDiscount(int id)
        {
            var userIdFromJwt = User.FindFirst("sub")?.Value;
            var roleFromJwt = User.FindFirst("role")?.Value ?? "Admin";

            var delete = await _taxDiscountService.DeleteTax(id, userIdFromJwt, roleFromJwt);
            return Ok(delete);
        }

    }
}
