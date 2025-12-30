using MultiTenantAuth.Application.Interfaces;
using MultiTenantAuth.Domain.Enums;

namespace MultiTenantAuth.Api.Services
{
    public class CurrentUserContext : ICurrentUserContext
    {
        public bool IsAuthenticated { get; internal set; }

        public string? UserId { get; internal set; }

        public UserScope UserScope { get; internal set; }

        public Guid? TenantId { get; internal set; }

        public List<string> Roles { get; internal set; } = new();

        public List<string> Permissions { get; internal set; } = new();

        IReadOnlyCollection<string> ICurrentUserContext.Roles => Roles;

        IReadOnlyCollection<string> ICurrentUserContext.Permissions => Permissions;
    }
}
