using System;
using System.Collections.Generic;

namespace Domain.Models;

public partial class Product
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public long BrandId { get; set; }

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public decimal Price { get; set; }

    public long ProductTypeId { get; set; }

    public virtual ProductBrand Brand { get; set; } = null!;

    public virtual ProductType ProductType { get; set; } = null!;
}
