using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class LogLogin
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public string? IpAddress { get; set; }

    public long ExpDate { get; set; }

    public bool IsSuccess { get; set; }

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual User User { get; set; } = null!;
}
