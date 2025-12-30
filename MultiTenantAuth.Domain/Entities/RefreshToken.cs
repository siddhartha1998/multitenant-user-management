using MultiTenantAuth.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenantAuth.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public UserScope UserScope { get; set; }

        public Guid? TenantId { get; set; }

        public string TokenHash { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }

        public DateTime CreatedAt { get; set; }
        public Guid? ReplacedByTokenId { get; set; }

        public string? DeviceInfo { get; set; }

        public ApplicationUser User { get; set; } = default!;
    }
}
