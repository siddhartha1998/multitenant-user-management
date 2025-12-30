using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MultiTenantAuth.Application.DTOs;
using MultiTenantAuth.Application.Interfaces;
using MultiTenantAuth.Domain.Entities;
using MultiTenantAuth.Domain.Enums;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenantAuth.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly JwtSettings _jwtSettings;
        public UsersController(UserManager<ApplicationUser> userManager,
                               ITokenService tokenService,
                               IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _jwtSettings = jwtSettings.Value;
          
        }

        [HttpPost("login")]
        public async Task<IActionResult> login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null || !user.IsActive ||
           !await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized();

            var accessToken = await _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Save or update the refresh token in the database

            // Set the refresh token as an HTTP-only cookie
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true in production
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            });

            return Ok(new
            {
                AccessToken = accessToken
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequest request)
        { 
            var oldRefreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(oldRefreshToken))
            {
                return Unauthorized("Refresh token is missing.");
            }
        
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if(principal == null)
            {
                return BadRequest("Invalid Token");
            }
            var username = principal.Identity?.Name;
            var savedRefreshToken = await _tokenRepository.GetRefreshToken(username, request.RefreshToken);

            if (savedRefreshToken == null || savedRefreshToken.IsRevoked || savedRefreshToken.ExpiryDate <= DateTime.UtcNow)
            {
                return Unauthorized("Invalid refresh token");
            }
            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            //await _tokenRepository.RevokeRefreshToken(savedRefreshToken);
            //await _tokenRepository.SaveRefreshToken(username, newRefreshToken);

            // Set the new refresh token as an HTTP-only cookie
            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Set to true in production
                Expires = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
            });

            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Remove the refresh token from the database
              //  RemoveRefreshToken(refreshToken);

                // Clear the HTTP-only cookie
                Response.Cookies.Delete("refreshToken");
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                return Ok(user.Id);
            }
            return BadRequest(result.Errors);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"])),
                ValidateLifetime = false // We want to get claims from expired token
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (!(securityToken is JwtSecurityToken jwtSecurityToken) ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        private RefreshToken CreateRefreshToken(ApplicationUser user)
        {
            var rawToken = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(64));

            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                UserScope = user.Scope,
                TenantId = user.Scope == UserScope.System ? null : user.TenantId,
                TokenHash = Hash(rawToken),
                ExpiresAt = DateTime.UtcNow.AddDays(14),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false,
              //  Token = rawToken // only returned once
            };
        }


    }

    public record CreateUserDto(string UserName, string Email, string Password);
    public record LoginRequest(string UserName, string Password);
    public record TokenRequest(string AccessToken, string RefreshToken);
}
