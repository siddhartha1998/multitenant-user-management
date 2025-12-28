using MultiTenantAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiTenantAuth.Application.Interfaces
{
    public interface ITenantService
    {
        Task<Tenant> CreateTenantAsync(string name, Guid tenantTypeId, Guid? parentTenantId);
        Task<TenantType> CreateTenantTypeAsync(string name, Guid? allowedParentTypeId);
        Task<IEnumerable<Tenant>> GetAllTenantsAsync();
        Task<IEnumerable<TenantType>> GetAllTenantTypesAsync();
        Task<Tenant?> GetTenantByIdAsync(Guid id);
    }
}
