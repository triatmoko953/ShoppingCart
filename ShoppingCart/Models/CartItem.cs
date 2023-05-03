using System;
using System.Collections.Generic;

namespace ShoppingCart.Models;

public partial class CartItem
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    public int Quantity { get; set; }

    public int CartUserId { get; set; }
}
