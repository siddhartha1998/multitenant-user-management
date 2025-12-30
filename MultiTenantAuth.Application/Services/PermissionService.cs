using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiTenantAuth.Application.DTOs;
using MultiTenantAuth.Application.Interfaces;
using MultiTenantAuth.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiTenantAuth.Application.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public PermissionService(IApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AssignPermissionToRoleAsync(Guid roleId, Guid permissionId)
        {
            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };
            _context.RolePermissions.Add(rolePermission);
            await _context.SaveChangesAsync();
        }

        public async Task AssignPermissionsToRoleAsync(Guid roleId, IEnumerable<Guid> permissionIds)
        {
            // 1. Fetch existing permissions for this role to avoid duplicates
            var existingPermissionIds = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            // 2. Filter out already assigned permissions
            var newPermissionIds = permissionIds.Distinct().Except(existingPermissionIds);

            // 3. Add new associations
            foreach (var permId in newPermissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permId
                });
            }

            await _context.SaveChangesAsync();
        }

        public async Task AssignRoleToUserAsync(Guid userId, Guid tenantId, Guid roleId)
        {
            // Validate Role exists and belongs to this tenant (or is global)
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                throw new ArgumentException($"Role with ID '{roleId}' not found.");
            }

            if (role.TenantId != null && role.TenantId != tenantId)
            {
                throw new ArgumentException("Cannot assign a role from a different tenant.");
            }

            //var tenantUser = await _context.TenantUsers
            //    .FirstOrDefaultAsync(tu => tu.UserId == userId && tu.TenantId == tenantId);

            //if (tenantUser == null)
            //{
            //    tenantUser = new TenantUser
            //    {
            //        Id = Guid.NewGuid(),
            //        UserId = userId,
            //        TenantId = tenantId
            //    };
            //    _context.TenantUsers.Add(tenantUser);
            //    await _context.SaveChangesAsync(); // Save to get ID
            //}

            //var tenantUserRole = new TenantUserRole
            //{
            //    TenantUserId = tenantUser.Id,
            //    RoleId = roleId
            //};
            //_context.TenantUserRoles.Add(tenantUserRole);
            await _context.SaveChangesAsync();
        }

        public async Task<Permission> CreatePermissionAsync(string name, string? description)
        {
            var permission = new Permission
            {
                Id = Guid.NewGuid(),
                Code = name,
                Description = description
            };
            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();
            return permission;
        }

        public async Task<ApplicationRole> CreateRoleAsync(string name, string? description, Guid? tenantId, bool isSystemRole)
        {
            // If tenantId is provided, namespace the internal name
            var internalName = tenantId.HasValue ? $"{tenantId.Value}_{name}" : name;
            
            var role = new ApplicationRole 
            { 
                Name = internalName, 
                Description = description,
                TenantId = tenantId,
                DisplayName = name, // Use the provided name as DisplayName
                IsSystemRole = isSystemRole
            };
            
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
            return role;
        }

        public async Task<ApplicationRole> CreateRoleWithPermissionAsync(CreateRoleWithPermissionDto dto)
        {
            var internalName = dto.TenantId.HasValue ? $"{dto.TenantId.Value}_{dto.Name}" : dto.Name;

            var role = new ApplicationRole
            {
                Name = internalName,
                Description = dto.Description,
                TenantId = dto.TenantId,
                DisplayName = dto.Name, // Use the provided name as DisplayName
                IsSystemRole = dto.IsSystemRole
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));

            foreach (var permissionId in dto.PermissionIds)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }

            await _context.SaveChangesAsync();
            return role;
        }
        public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, Guid tenantId)
        {
            // 1. Get TenantUser record
            //var tenantUser = await _context.TenantUsers
            //    .Include(tu => tu.TenantUserRoles)
            //        .ThenInclude(tur => tur.Role)
            //            .ThenInclude(r => r.RolePermissions)
            //                .ThenInclude(rp => rp.Permission)
            //    .FirstOrDefaultAsync(tu => tu.UserId == userId && tu.TenantId == tenantId);

            //if (tenantUser == null)
            //{
            //    return Enumerable.Empty<string>();
            //}

            //// 2. Aggregate permissions from all roles assigned to user in this tenant
            //var permissions = tenantUser.TenantUserRoles
            //    .SelectMany(tur => tur.Role.RolePermissions)
            //    .Select(rp => rp.Permission.Code)
            //    .Distinct()
            //    .ToList();

            var permissions = new List<string>();

            return permissions;
        }
    }
}
