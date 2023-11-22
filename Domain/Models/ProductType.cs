using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class ProductType
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
