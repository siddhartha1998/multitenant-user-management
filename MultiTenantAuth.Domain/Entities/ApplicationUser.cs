using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace MultiTenantAuth.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public Guid? MerchantId { get; set; }
        public Merchant Merchant { get; set; }

        public Guid? OutletId { get; set; }
        public Outlet Outlet { get; set; }

        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }

        public ICollection<ApplicationRole> TenantUserRoles { get; set; } = new List<ApplicationRole>();

        public bool IsMerchantUser => MerchantId.HasValue && !OutletId.HasValue;
        public bool IsOutletUser => OutletId.HasValue;
    }
}
