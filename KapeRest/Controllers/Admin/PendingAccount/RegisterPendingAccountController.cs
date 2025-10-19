using KapeRest.Application.Services.Admin.PendingAcc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KapeRest.DTOs.Admin.PendingAccount;
using KapeRest.Application.DTOs.Admin.PendingAccount;
using Microsoft.AspNetCore.Authorization;

namespace KapeRest.Controllers.Admin.PendingAccount
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RegisterPendingAccountController : ControllerBase
    {
        private readonly PendingAccService _pendingAccService;
        public RegisterPendingAccountController(PendingAccService pendingAccService)
        {
            _pendingAccService = pendingAccService;
        }

        [HttpPost("RegisterPendingAccount")]
        public async Task<ActionResult> RegisterPendingAccount(API_PendingAccount pending)
        {
            var pendingAccDTO = new PendingAccDTO
            {
                FirstName = pending.FirstName,
                MiddleName = pending.MiddleName,
                LastName = pending.LastName,
                Email = pending.Email,
                Password = pending.Password,
                Role = pending.Role,
            };
            await _pendingAccService.RegisterPending(pendingAccDTO);
            return Ok("Successfully pending your account");
        }

        [HttpPost("ApprovePendingAccount/{id}")]
        public async Task<ActionResult> ApprovePendingAccount(int id)
        {
            await _pendingAccService.ApprovePendingAccount(id);
            return Ok("Successfully approved the pending account.");
        }

        [HttpPost("RejectPendingAccount/{id}")]
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

    }

}