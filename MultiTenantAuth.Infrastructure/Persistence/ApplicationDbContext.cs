using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenantAuth.Domain.Entities;
using MultiTenantAuth.Application.Interfaces;
using System;

namespace MultiTenantAuth.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<TenantType> TenantTypes { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // TenantType Hierarchy
            builder.Entity<TenantType>()
                .HasOne(tt => tt.AllowedParentType)
                .WithMany()
                .HasForeignKey(tt => tt.AllowedParentTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Tenant Hierarchy
            builder.Entity<Tenant>()
                .HasOne(t => t.TenantType)
                .WithMany(tt => tt.Tenants)
                .HasForeignKey(t => t.TenantTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Tenant>()
                .HasOne(t => t.ParentTenant)
                .WithMany(pt => pt.ChildTenants)
                .HasForeignKey(t => t.ParentTenantId)
                .OnDelete(DeleteBehavior.Restrict);

            // RolePermission (Many-to-Many)
            builder.Entity<RolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId);

            builder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            // TenantUser
            //builder.Entity<TenantUser>()
            //    .HasOne(tu => tu.Tenant)
            //    .WithMany(t => t.TenantUsers)
            //    .HasForeignKey(tu => tu.TenantId);

            //builder.Entity<TenantUser>()
            //    .HasOne(tu => tu.User)
            //    .WithMany(u => u.TenantUsers)
            //    .HasForeignKey(tu => tu.UserId);

            //// TenantUserRole
            //builder.Entity<TenantUserRole>()
            //    .HasKey(tur => new { tur.TenantUserId, tur.RoleId });

            //builder.Entity<TenantUserRole>()
            //    .HasOne(tur => tur.TenantUser)
            //    .WithMany(tu => tu.TenantUserRoles)
            //    .HasForeignKey(tur => tur.TenantUserId);

            //builder.Entity<TenantUserRole>()
            //    .HasOne(tur => tur.Role)
            //    .WithMany(r => r.TenantUserRoles)
            //    .HasForeignKey(tur => tur.RoleId);
        }
    }
}
