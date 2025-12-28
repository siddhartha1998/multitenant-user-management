using System;

namespace MultiTenantAuth.Domain.Entities
{
    public class Merchant
    {
        public Guid Id { get; set; }
        public string LegalName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string BusinessCode { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public bool IsActive { get; set; }
        public Guid TenantId { get; set; }
        public Tenant Tenant { get; set; } = null!;

        public ICollection<Outlet> Outlets { get; set; } = new List<Outlet>();
        public ICollection<ApplicationUser> TenantUsers { get; set; } = new List<ApplicationUser>();
    }
}
