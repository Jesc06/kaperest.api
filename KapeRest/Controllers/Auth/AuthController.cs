using KapeRest.Application.DTOs.Auth;
using KapeRest.Application.DTOs.Jwt;
using KapeRest.Application.Services.Auth;
using KapeRest.DTOs.Auth;
using KapeRest.DTOs.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KapeRest.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AccountService _accountService;
        public AuthController(AccountService accountService)
        {
            _accountService = accountService;
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

        [HttpPost("ChangePassword")]
        public async Task<ActionResult>ChangePassword(API_ChangePass pass)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var changePassDTO = new ChangePassDTO
            {
                Email = pass.Email,
                CurrentPassword = pass.CurrentPassword,
                NewPassword = pass.NewPassword
            };

            await _accountService.ChangePassword(changePassDTO);
            return Ok("Password changed successfully.");
        }


    }
}
