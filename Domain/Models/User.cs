using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class User
{
    public long Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual ICollection<UserRole> UserRoleRoles { get; } = new List<UserRole>();

    public virtual ICollection<UserRole> UserRoleUsers { get; } = new List<UserRole>();
}
