using Microsoft.Extensions.DependencyInjection;
using MultiTenantAuth.Application.Interfaces;
using MultiTenantAuth.Application.Services;

namespace MultiTenantAuth.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITenantService, TenantService>();
            services.AddScoped<IPermissionService, PermissionService>();
            return services;
        }
    }
}
