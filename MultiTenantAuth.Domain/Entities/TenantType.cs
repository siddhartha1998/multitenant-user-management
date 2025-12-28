using System;
using System.Collections.Generic;

namespace MultiTenantAuth.Domain.Entities
{
    public class TenantType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., MSP, BANK, BRANCH
        public string DisplayName { get; set; } = string.Empty; //e.g.Manage Service Provider, Bank, Branch etc.

        // Hierarchy definition
        public Guid? AllowedParentTypeId { get; set; }
        public TenantType? AllowedParentType { get; set; }
        
        public ICollection<Tenant> Tenants { get; set; } = new List<Tenant>();
    }
}
