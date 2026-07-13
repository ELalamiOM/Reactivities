using Application.Account.Commands;
using Application.Account.Common;
using Application.Account.DTOs;
using Application.Account.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto)
        {
            var result = await Mediator.Send(new RegisterUser.Command { RegisterDto = registerDto });
            return HandleAuthResult(result);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var result = await Mediator.Send(new LoginUser.Command { LoginDto = loginDto });
            return HandleAuthResult(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized("Refresh token is missing");
            }

            var result = await Mediator.Send(new RefreshUserToken.Command { RefreshToken = refreshToken });
            return HandleAuthResult(result);
        }

        [AllowAnonymous]
        [HttpGet("user-info")]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return NoContent();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await Mediator.Send(new GetCurrentUserInfo.Query { UserId = userId });
            return HandleAuthResult(result);
        }
        
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            var result = await Mediator.Send(new LogoutUser.Command { RefreshToken = refreshToken });
            if (!result.IsSuccess)
                return BadRequest(result.Error);

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return NoContent();
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            var result = await Mediator.Send(new ForgotPassword.Command { ForgotPasswordDto = forgotPasswordDto });
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            return Ok();
        }

        private ActionResult<UserDto> HandleAuthResult(Application.Core.Result<AccountAuthResult> result)
        {
            if (!result.IsSuccess && result.Code == 401)
                return Unauthorized(result.Error);
            if (!result.IsSuccess)
                return BadRequest(result.Error);
            if (result.Value == null)
                return BadRequest("Invalid authentication result");

            Response.Cookies.Append("refreshToken", result.Value.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = result.Value.RefreshTokenExpiresAt,
                Path = "/"
            });

            return Ok(result.Value.User);
        }
    }
}