using KapeRest.Application.DTOs.Admin.TaxDiscount;
using KapeRest.Application.Services.Admin.TaxDiscount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualBasic;

namespace KapeRest.Api.Controllers.Admin.TaxDiscount
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly TaxDiscountService _taxDiscountService;
        public DiscountController(TaxDiscountService taxDiscountService)
        {
            _taxDiscountService = taxDiscountService;
        }

        [HttpPost("AddDiscount")]
        public async Task<ActionResult> AddDiscount(TaxDiscountDTO dto)
        {
            var result = await _taxDiscountService.AddDiscount(dto);
            return Ok(result);
        }

        [HttpPut("UpdateDiscount")]
        public async Task<ActionResult> UpdateDiscount(UpdateTaxDiscountDTO dto)
        {
            var userIdFromJwt = User.FindFirst("uid")?.Value;
            var roleFromJwt = User.FindFirst("role")?.Value ?? "Admin";
            var result = await _taxDiscountService.UpdateDiscount(dto,userIdFromJwt,roleFromJwt);
            return Ok(result);
        }

        [HttpDelete("DeleteDiscount")]
        public async Task<ActionResult> DeleteDiscount(int id)
        {
            var result = await _taxDiscountService.DeleteDiscount(id);
            return Ok(result);
        }

        [HttpGet("GetALlDiscount")]
        public async Task<ActionResult> GetAllDiscount()
        {
            var result = await _taxDiscountService.GetAllDiscount();
            return Ok(result);
        }

    }
}
