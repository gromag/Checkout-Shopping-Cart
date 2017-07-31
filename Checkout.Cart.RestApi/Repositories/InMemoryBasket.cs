using System;
using Checkout.Cart.Abstracts;
using System.Collections.Generic;
using Checkout.Cart.Models;
using System.Linq;

namespace Checkout.Cart.Repositories
{
    /// <summary>
    /// Marked as internal as this should be visible only from 
    /// within Checkout.Cart.Repositories 
    /// </summary>
    internal class InMemoryBasket : Dictionary<Guid, InMemoryItems> {

    }
}
