using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class RoleParent
{
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long ParentId { get; set; }

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual Role Parent { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
