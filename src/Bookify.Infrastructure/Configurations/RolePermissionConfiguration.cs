using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");

/*
* Composite key to have: RoleId : PermissionId
* Ensure that there won't be duplicate keys.
| RoleId | PermissionId |
|--------|------------- |
| 1      | 1            |  ← Registered → users:read  ✅
| 2      | 1            |  ← Admin → users:read       ✅ (different RoleId, allowed!)
| 1      | 1            |  ← Duplicate!               ❌ (same pair)
*/
        builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionId });

        builder.HasData(
            new RolePermission
            {
                RoleId = Role.Registered.Id,
                PermissionId = Permission.UsersRead.Id
            });
    }
}
