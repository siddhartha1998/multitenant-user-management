using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MultiTenantAuth.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
      //  public ICollection<TenantUserRole> TenantUserRoles { get; set; } = new List<TenantUserRole>();
    }
}
