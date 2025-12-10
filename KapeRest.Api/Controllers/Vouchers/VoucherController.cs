using KapeRest.Application.DTOs.Vouchers;
using KapeRest.Application.Interfaces.Vouchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace KapeRest.Api.Controllers.Vouchers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IVoucherService voucherService)
        {
            _voucherService = voucherService;
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CreateVoucher(CreateVoucherDTO createVoucher)
        {
            try
            {
                var adminId = User.FindFirst("adminId")?.Value ?? "System";
                var voucher = await _voucherService.CreateVoucherAsync(createVoucher, adminId);
                return Ok(new { message = "Voucher created successfully", voucher });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("Validate")]
        public async Task<ActionResult> ValidateVoucher([FromBody] ValidateVoucherDTO validateDto)
        {
            try
            {
                var result = await _voucherService.ValidateVoucherAsync(validateDto.Code, validateDto.OrderAmount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllVouchers()
        {
            try
            {
                var vouchers = await _voucherService.GetAllVouchersAsync();
                return Ok(vouchers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetActive")]
        public async Task<ActionResult> GetActiveVouchers()
        {
            try
            {
                var vouchers = await _voucherService.GetActiveVouchersAsync();
                return Ok(vouchers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("GetByCode/{code}")]
        public async Task<ActionResult> GetVoucherByCode(string code)
        {
            try
            {
                var voucher = await _voucherService.GetVoucherByCodeAsync(code);
                if (voucher == null)
                {
                    return NotFound(new { error = "Voucher not found" });
                }
                return Ok(voucher);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("Deactivate/{voucherId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateVoucher(int voucherId)
        {
            try
            {
                var success = await _voucherService.DeactivateVoucherAsync(voucherId);
                if (!success)
                {
                    return NotFound(new { error = "Voucher not found" });
                }
                return Ok(new { message = "Voucher deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("UsageHistory/{voucherId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetVoucherUsageHistory(int voucherId)
        {
            try
            {
                var usages = await _voucherService.GetVoucherUsageHistoryAsync(voucherId);
                return Ok(usages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("AllUsages")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> GetAllVoucherUsages()
        {
            try
            {
                var usages = await _voucherService.GetAllVoucherUsagesAsync();
                return Ok(usages);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
