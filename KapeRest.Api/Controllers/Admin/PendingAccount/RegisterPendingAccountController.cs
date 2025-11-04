using KapeRest.Application.DTOs.Admin.PendingAccount;
using KapeRest.Application.Services.Admin.PendingAcc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KapeRest.Controllers.Admin.PendingAccount
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class RegisterPendingAccountController : ControllerBase
    {
        private readonly PendingAccService _pendingAccService;
        public RegisterPendingAccountController(PendingAccService pendingAccService)
        {
            _pendingAccService = pendingAccService;
        }

        [HttpPost("RegisterPendingAccount")]
        public async Task<ActionResult> RegisterPendingAccount(PendingAccDTO pending)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _pendingAccService.RegisterPending(pending);
            return Ok(result);
        }

        [HttpPost("ApprovePendingAccount/{id}")]
        public async Task<ActionResult> ApprovePendingAccount(int id)
        {
            await _pendingAccService.ApprovePendingAccount(id);
            return Ok("Successfully approved the pending account.");
        }

        [HttpDelete("RejectPendingAccount/{id}")]
        public async Task<ActionResult> RejectPendingAccount(int id)
        {
            await _pendingAccService.RejectPendingAccount(id);
            return Ok("Successfully rejected the pending account.");
        }

        [HttpGet("GetAllPendingAccounts")]
        public async Task<ActionResult> GetAllPendingAccounts()
        {
            var pending = await _pendingAccService.GetPendingAccounts();
            return Ok(pending);
        }

        [HttpGet("ExistingCashierAccount")]
        public async Task<ActionResult> ExistingCashierAccount()
        {
            var result = await _pendingAccService.ExistingCashierAccountNavigaiton();
            return Ok(result);
        }



    }

}