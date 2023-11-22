using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class ProductBrand
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public long? Ddate { get; set; }

    public long? DuserId { get; set; }

    public long? Cdate { get; set; }

    public long? CuserId { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
