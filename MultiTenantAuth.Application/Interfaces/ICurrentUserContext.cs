using MultiTenantAuth.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiTenantAuth.Application.Interfaces
{
    public interface ICurrentUserContext
    {
        bool IsAuthenticated { get; }
        string? UserId  { get; }
        UserScope UserScope { get; }
        Guid? TenantId { get; }
        IReadOnlyCollection<string> Roles { get; }
        IReadOnlyCollection<string> Permissions { get; }
    }
}
