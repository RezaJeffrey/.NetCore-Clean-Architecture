using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Region
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public int RegCode { get; set; }

    public long CityId { get; set; }

    public long? CreateDate { get; set; }

    public long? CreateUserId { get; set; }

    public long? ModifyDate { get; set; }

    public long? ModifyUserId { get; set; }

    public long? DeleteDate { get; set; }

    public long? DeleteUserId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
