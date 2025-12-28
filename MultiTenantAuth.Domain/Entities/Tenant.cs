using System;
using System.Collections.Generic;

namespace MultiTenantAuth.Domain.Entities
{
    public class Tenant
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        public Guid TenantTypeId { get; set; }
        public TenantType TenantType { get; set; } = null!;
        
        public Guid? ParentTenantId { get; set; }
        public Tenant? ParentTenant { get; set; }

        public ICollection<Tenant> ChildTenants { get; set; } = new List<Tenant>();
        public ICollection<ApplicationUser> TenantUsers { get; set; } = new List<ApplicationUser>();
        public ICollection<ApplicationRole> Roles { get; set; } = new List<ApplicationRole>();
        public ICollection<Merchant> Merchants { get; set; } = new List<Merchant>();
    }
}
