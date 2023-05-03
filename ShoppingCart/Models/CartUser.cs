using System;
using System.Collections.Generic;

namespace ShoppingCart.Models;

public partial class CartUser
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public bool Checkout { get; set; }
}
