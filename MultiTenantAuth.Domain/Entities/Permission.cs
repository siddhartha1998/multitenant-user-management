using System;
using System.Collections.Generic;

namespace MultiTenantAuth.Domain.Entities
{
    public class Permission
    {
        public Guid Id { get; set; }
        public string SystemCode { get; set; } = string.Empty; // TMS, POS, IDENTITY, RKI
        public string Code { get; set; } = string.Empty; // e.g. IDENTITY.USER_CREATE, POS_CREATE_DEPLOYMENT_REQUEST etc.
        public string? Description { get; set; }
        
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
