using System;

namespace MultiTenantAuth.Domain.Entities
{
    public class Outlet
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OutletCode { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; } = null!;
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
