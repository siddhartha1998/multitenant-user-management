using Microsoft.EntityFrameworkCore;
using MultiTenantAuth.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace MultiTenantAuth.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Tenant> Tenants { get; }
        DbSet<TenantType> TenantTypes { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<RolePermission> RolePermissions { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
