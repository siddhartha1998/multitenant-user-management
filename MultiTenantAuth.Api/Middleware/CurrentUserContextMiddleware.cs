using Microsoft.AspNetCore.Identity;
using MultiTenantAuth.Api.Services;
using MultiTenantAuth.Domain.Entities;
using MultiTenantAuth.Domain.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MultiTenantAuth.Api.Middleware
{
    public class CurrentUserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext,
                                 CurrentUserContext currentUser,
                                 UserManager<ApplicationUser> userManager)
        {
            var user = httpContext.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                currentUser.IsAuthenticated = true;
                currentUser.UserId = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

                var scopeClaim = user.FindFirst("user_scope");
                if (scopeClaim != null)
                    currentUser.UserScope = Enum.Parse<UserScope>(scopeClaim.Value);

                var tenantClaim = user.FindFirst("tid");
                if (tenantClaim != null)
                    currentUser.TenantId = Guid.Parse(tenantClaim.Value);

                var appUser = await userManager.FindByIdAsync(currentUser.UserId!);
                var roles = await userManager.GetRolesAsync(appUser!);
                currentUser.Roles.AddRange(roles);

                //var permissions = await rolePermissionService.GetPermissionsAsync(roles);
                //currentUser.Permissions.AddRange(permissions);
            }

            await _next(httpContext);
        }
    }
}
