using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(SignInManager<User> signInManager, ITokenService tokenService) : BaseApiController
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto)
        {
            var user = new User
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName
            };

            var result = await signInManager.UserManager.CreateAsync(user,registerDto.Password);

            if(result.Succeeded) return Ok(await CreateUserObject(user));

            foreach( var error in result.Errors)
            {
                ModelState.AddModelError(error.Code,error.Description);
            }
            return ValidationProblem();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await signInManager.UserManager.FindByEmailAsync(loginDto.Email);
            if (user == null) return Unauthorized("Invalid email or password");

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid email or password");

            return Ok(await CreateUserObject(user));
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
                return Unauthorized("Refresh token is missing");

            var user = await signInManager.UserManager.Users
                .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
                return Unauthorized("Refresh token is invalid or expired");

            return Ok(await CreateUserObject(user));
        }

        [AllowAnonymous]
        [HttpGet("user-info")]
        public async Task<ActionResult<UserDto>> GetUserInfo()
        {
             if(User.Identity?.IsAuthenticated == false) return NoContent();

             var user = await signInManager.UserManager.GetUserAsync(User);

             if(user == null) return Unauthorized();

             return Ok(await CreateUserObject(user));
        }
        
        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken) && !string.IsNullOrWhiteSpace(refreshToken))
            {
                var user = await signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiresAt = DateTime.UtcNow;
                    var updateResult = await signInManager.UserManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                        return BadRequest("Could not revoke refresh token");
                }
            }

            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

            return NoContent();
        }

        private async Task<UserDto> CreateUserObject(User user)
        {
            user.RefreshToken = tokenService.GenerateRefreshToken();
            user.RefreshTokenExpiresAt = tokenService.GetRefreshTokenExpiryDate();

            var updateResult = await signInManager.UserManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new InvalidOperationException("Could not persist refresh token for the user");

            Response.Cookies.Append("refreshToken", user.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = user.RefreshTokenExpiresAt,
                Path = "/"
            });

            return new UserDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ImageUrl = user.ImageUrl,
                Token = tokenService.CreateToken(user)
            };
        }
    }
}