using System;
using Checkout.Cart.Models;

namespace Checkout.Cart.Abstracts
{
    public interface ICartRepository
    {
        Basket New();
        bool Add(Guid cartId, Item item);
        bool Delete(Guid cartId, string name);
        Basket Read(Guid cartId);
        Basket Read(Guid cartId, string name);
        bool Update(Guid cartId, Item item);
    }
}
