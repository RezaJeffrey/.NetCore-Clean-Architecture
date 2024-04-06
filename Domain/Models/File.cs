using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class File
{
    public long Id { get; set; }

    public string FileName { get; set; } = null!;

    public string? EntityType { get; set; }

    public long? EntityId { get; set; }

    public string? Url { get; set; }

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual Product? Entity { get; set; }

    public virtual Wftransition? Entity1 { get; set; }

    public virtual Project? EntityNavigation { get; set; }
}
