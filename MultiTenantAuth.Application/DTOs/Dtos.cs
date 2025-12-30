using System;

namespace MultiTenantAuth.Application.DTOs
{
    public record CreateTenantTypeDto(string Name, Guid? AllowedParentTypeId);
    public record CreateTenantDto(string Name, Guid TenantTypeId, Guid? ParentTenantId);
    public record CreateRoleDto(string Name,bool IsSystemRole, string? Description, Guid? TenantId);
    public record CreateRoleWithPermissionDto(string Name,bool IsSystemRole, string? Description, Guid? TenantId, IEnumerable<Guid> PermissionIds);
    public record CreatePermissionDto(string Name, string? Description);
    public record AssignRoleDto(Guid UserId, Guid TenantId, Guid RoleId);
    public record AssignPermissionDto(Guid RoleId, Guid PermissionId);
    public record AssignPermissionsDto(Guid RoleId, List<Guid> PermissionIds);
}
