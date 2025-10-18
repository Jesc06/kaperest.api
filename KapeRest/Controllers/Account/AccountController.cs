using KapeRest.Application.DTOs.Account;
using KapeRest.Application.DTOs.Jwt;
using KapeRest.Application.Services.Account;
using KapeRest.DTOs.Account;
using KapeRest.DTOs.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
        public async Task<IActionResult> RegisterAccount([FromBody]API_RegisterAccountDTO register)
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
            return Created();
        }

        [HttpPost("Login")]
        public async Task<ActionResult<JwtTokenDTO>> Login([FromBody]API_LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loginAccountDTO = new LoginDTO
            {
                Email = login.Email,
                Password = login.Password
            };

            var result = await _accountService.Login(loginAccountDTO);

            var tokenDTO = new CreateJwtTokenDTO
            {
                Token = result.Token,
                RefreshToken = result.RefreshToken
            };

            if (result is not null)
            {
                return Ok(tokenDTO);
            }
            return BadRequest("Invalid credentials");
        }


        [HttpPost("RefreshToken")]
        public async Task<ActionResult<JwtRefreshResponseDTO>> RefreshToken([FromBody]JwtRefreshToken refreshToken)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var tokenRequest = new JwtRefreshRequestDTO
            {
                Token = refreshToken.Token,
                RefreshToken = refreshToken.RefreshToken
            };
            var result = await _accountService.TokenRefresh(tokenRequest);
            if (result is null)
                return Unauthorized(new { message = "Invalid or expired refresh token" });

            return Ok(result);
        }


        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _accountService.Logout();
            return Ok("Logged out successfully.");
        }

    }
}
