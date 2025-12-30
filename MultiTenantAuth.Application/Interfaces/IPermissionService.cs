using MultiTenantAuth.Application.DTOs;
using MultiTenantAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiTenantAuth.Application.Interfaces
{
    public interface IPermissionService
    {
        Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, Guid tenantId);
        Task AssignRoleToUserAsync(Guid userId, Guid tenantId, Guid roleId);
        Task<ApplicationRole> CreateRoleAsync(string name, string? description, Guid? tenantId, bool isSystemRole);
        Task<ApplicationRole> CreateRoleWithPermissionAsync(CreateRoleWithPermissionDto dto);
        Task<Permission> CreatePermissionAsync(string name, string? description);
        Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId);
        Task AssignPermissionsToRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds);
    }
}
