using System.Collections.Generic;
using Checkout.Cart.Models;

namespace Checkout.Cart.Repositories
{
    /// <summary>
    /// Marked as internal as this should be visible only from 
    /// within Checkout.Cart.Repositories 
    /// </summary>
    internal class InMemoryItems : Dictionary<string, Item> { }

}
