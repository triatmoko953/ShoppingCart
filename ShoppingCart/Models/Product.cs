using System;
using System.Collections.Generic;

namespace ShoppingCart.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public int Stock { get; set; }

    public bool Deleted { get; set; }
}
