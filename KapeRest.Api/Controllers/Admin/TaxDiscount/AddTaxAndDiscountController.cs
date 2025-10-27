using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Services.Admin.TaxDiscount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KapeRest.Api.Controllers.Admin.TaxDiscount
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddTaxAndDiscountController : ControllerBase
    {
        private readonly TaxDiscountService _taxDiscountService;
        public AddTaxAndDiscountController(TaxDiscountService taxDiscountService)
        {
            _taxDiscountService = taxDiscountService;
        }

        [HttpPost("AddTaxAndDiscount")]
        public async Task<ActionResult> AddTaxAndDiscount(TaxDiscountDTO dto)
        {
            var add = await _taxDiscountService.TaxAndDiscount(dto);
            return Ok(add);
        }

        [HttpPut("UpdateTaxAndDiscount")]
        public async Task<ActionResult> UpdateTaxAndDiscount(UpdateTaxDiscountDTO dto)
        {
            var update = await _taxDiscountService.UpdateTaxDiscount(dto);
            return Ok(update);
        }

        [HttpGet("GetAllTaxAndDiscount")]
        public async Task<ActionResult> GetAllTaxAndDiscount()
        {
            var getAll = await _taxDiscountService.GetAllTaxAndDiscount();
            return Ok(getAll);
        }

        [HttpDelete("DeleteTaxAndDiscount")]
        public async Task<ActionResult> DeleteTaxAndDiscount(int id)
        {
            var delete = await _taxDiscountService.DeleteTaxAndDiscount(id);
            return Ok(delete);
        }

    }
}
