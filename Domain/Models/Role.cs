using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Role
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public int Gcode { get; set; }

    public string? Description { get; set; }

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual ICollection<RegisterRequest> RegisterRequests { get; set; } = new List<RegisterRequest>();

    public virtual ICollection<RoleParent> RoleParentParents { get; set; } = new List<RoleParent>();

    public virtual ICollection<RoleParent> RoleParentRoles { get; set; } = new List<RoleParent>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
