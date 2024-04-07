using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Role
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public int Gcode { get; set; }

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
