using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Checkout.Cart.Abstracts;

namespace Checkout.Cart.Models
{
    public class Basket
    {
        public Guid Id { get; set; }
        public ICollection<Item> Items { get; set; }
    }
}
