using Application.Account.Commands;
using Application.Account.Common;
using Application.Account.DTOs;
using Application.Account.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    // Contrôleur de gestion des comptes utilisateurs
    // Gère l'authentification, l'inscription, la déconnexion et la gestion des tokens
    public class AccountController : BaseApiController
    {
        // POST: api/account/register
        // Inscription d'un nouvel utilisateur - Route publique
        // Crée un compte avec email, mot de passe et nom d'affichage
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterUser(RegisterDto registerDto)
        {
            var result = await Mediator.Send(new RegisterUser.Command { RegisterDto = registerDto });
            return HandleAuthResult(result);
        }

        // POST: api/account/login
        // Connexion d'un utilisateur existant - Route publique
        // Authentifie avec email et mot de passe, retourne un token JWT
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var result = await Mediator.Send(new LoginUser.Command { LoginDto = loginDto });
            return HandleAuthResult(result);
        }

        // POST: api/account/refresh-token
        // Rafraîchit le token JWT via le refresh token en cookie - Route publique
        // Permet de prolonger la session sans redemander le mot de passe
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

        // GET: api/account/user-info
        // Récupère les infos de l'utilisateur connecté - Route publique
        // Retourne NoContent si non authentifié, utilisé au chargement de l'app
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

        // POST: api/account/logout
        // Déconnexion de l'utilisateur - Route protégée (authentification requise)
        // Révoque le refresh token et supprime le cookie de session
        [Authorize]
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

        // POST: api/account/forgot-password
        // Demande de réinitialisation du mot de passe - Route publique
        // Retourne toujours OK pour éviter l'énumération d'emails (sécurité)
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