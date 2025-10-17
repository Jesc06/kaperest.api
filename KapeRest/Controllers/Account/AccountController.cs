using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KapeRest.Application.Services.Account;
using KapeRest.DTOs.Account;
using KapeRest.Application.DTOs.Account;

namespace KapeRest.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("RegisterAccount")]
        public async Task<IActionResult> RegisterAccount(API_RegisterAccountDTO register)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var registeredAccount = new RegisterAccountDTO
            {
                FirstName = register.FirstName,
                MiddleName = register.MiddleName,
                LastName = register.LastName,
                Email = register.Email,
                Password = register.Password,
                Roles = register.Roles
            };
            var result = await _accountService.RegisterAccountService(registeredAccount);
            return Ok(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(API_LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loginAccountDTO = new LoginDTO
            {
                Email = login.Email,
                Password = login.Password
            };

            var result = await _accountService.Login(loginAccountDTO);
            if (result)
            {
                return Ok("Login Successfully");
            }
            return BadRequest("Invalid credentials");

        }


    }
}
