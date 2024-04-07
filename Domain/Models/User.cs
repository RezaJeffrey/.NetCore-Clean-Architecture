using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class User
{
    public long Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? Name { get; set; }

    public long? RoleId { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual ICollection<LogLogin> LogLogins { get; set; } = new List<LogLogin>();

    public virtual Role? Role { get; set; }
}
