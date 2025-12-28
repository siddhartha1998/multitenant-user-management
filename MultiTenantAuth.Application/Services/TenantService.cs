using Microsoft.EntityFrameworkCore;
using MultiTenantAuth.Application.Interfaces;
using MultiTenantAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiTenantAuth.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly IApplicationDbContext _context;

        public TenantService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TenantType> CreateTenantTypeAsync(string name, Guid? allowedParentTypeId)
        {
            if (await _context.TenantTypes.AnyAsync(tt => tt.Name == name))
            {
                throw new ArgumentException($"Tenant Type with name '{name}' already exists.");
            }

            var tenantType = new TenantType
            {
                Id = Guid.NewGuid(),
                Name = name,
                AllowedParentTypeId = allowedParentTypeId
            };

            _context.TenantTypes.Add(tenantType);
            await _context.SaveChangesAsync();
            return tenantType;
        }

        public async Task<Tenant> CreateTenantAsync(string name, Guid tenantTypeId, Guid? parentTenantId)
        {
            // Check for duplicate name within the same parent (or duplicate root name if parent is null)
            if (await _context.Tenants.AnyAsync(t => t.Name == name && t.ParentTenantId == parentTenantId))
            {
                var scope = parentTenantId.HasValue ? "under this parent" : "at root level";
                throw new ArgumentException($"Tenant with name '{name}' already exists {scope}.");
            }

            // Validate Hierarchy
            var tenantType = await _context.TenantTypes.FindAsync(tenantTypeId);
            if (tenantType == null)
            {
                throw new ArgumentException("Invalid Tenant Type");
            }

            if (tenantType.AllowedParentTypeId.HasValue)
            {
                if (!parentTenantId.HasValue)
                {
                    throw new ArgumentException($"Tenant of type {tenantType.Name} requires a parent.");
                }

                var parentTenant = await _context.Tenants.Include(t => t.TenantType).FirstOrDefaultAsync(t => t.Id == parentTenantId.Value);
                if (parentTenant == null)
                {
                    throw new ArgumentException("Parent Tenant not found.");
                }

                if (parentTenant.TenantTypeId != tenantType.AllowedParentTypeId.Value)
                {
                    throw new ArgumentException($"Tenant of type {tenantType.Name} must be a child of {tenantType.AllowedParentType?.Name ?? "Unknown"}.");
                }
            }
            else
            {
                if (parentTenantId.HasValue)
                {
                    throw new ArgumentException($"Tenant of type {tenantType.Name} cannot have a parent.");
                }
            }

            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = name,
                TenantTypeId = tenantTypeId,
                ParentTenantId = parentTenantId
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();
            return tenant;
        }

        public async Task<IEnumerable<Tenant>> GetAllTenantsAsync()
        {
            return await _context.Tenants
                .Include(t => t.TenantType)
                .Include(t => t.ParentTenant)
                .ToListAsync();
        }

        public async Task<IEnumerable<TenantType>> GetAllTenantTypesAsync()
        {
            return await _context.TenantTypes.ToListAsync();
        }

        public async Task<Tenant?> GetTenantByIdAsync(Guid id)
        {
            return await _context.Tenants
                .Include(t => t.TenantType)
                .Include(t => t.ParentTenant)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
