using Microsoft.EntityFrameworkCore;
using MultiTenantAuth.Application.Interfaces;
using MultiTenantAuth.Infrastructure.Persistence;

namespace MultiTenantAuth.Api.Services
{
    public class RolePermissionService : IRolePermisssionService
    {
        private readonly ApplicationDbContext _dbContext;

        public RolePermissionService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IReadOnlyCollection<string>> GetPermissionsAsync(List<string> roles)
        {
            var permission = await _dbContext.RolePermissions
            .Where(rp => roles.Contains(rp.RoleId.ToString()))
            .Select(rp => rp.Permission.Code)
            .ToListAsync();

            return permission;
        }
    }
}
