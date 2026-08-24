namespace Bookify.Domain.Users;

/*
 * For many to many relationship
 * Join Entity, so we wont need Navigation property
 */
public sealed class RolePermission
{
    public int RoleId { get; set; }

    public int PermissionId { get; set; }
}
