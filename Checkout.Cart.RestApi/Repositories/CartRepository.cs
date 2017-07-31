using System;
using System.Linq;
using System.Linq.Expressions;
using Checkout.Cart.Abstracts;
using System.Collections.Generic;
using Checkout.Cart.Models;

namespace Checkout.Cart.Repositories
{
    public partial class CartRepository : ICartRepository
    {

        private InMemoryBasket _inMemoryBasket;
        /// <summary>
        /// All operations that touch the basket in multiple steps
        /// are guaranteed to execute in sequence by the lock and
        /// no concurrent request is able to modify the basket 
        /// before the 'unit of work' operation have completed.
        /// </summary>
        private static object Lock = new object();

        public CartRepository()
        {
            _inMemoryBasket = new InMemoryBasket();
        }

        public Basket New()
        {
            lock (Lock)
            {
                var id = Guid.NewGuid();

                _inMemoryBasket.Add(id, new InMemoryItems());

                return ToBasket(id);
            }
        }

        public bool Add(Guid cartId, Item item)
        {
            lock (Lock)
            {
                if (!_inMemoryBasket.ContainsKey(cartId))
                {
                    return false;
                }

                var itemsCollection = _inMemoryBasket[cartId];

                if (!itemsCollection.ContainsKey(item.Name))
                {
                    itemsCollection.Add(item.Name, new Item { Name = item.Name });
                }

                itemsCollection[item.Name].Quantity += item.Quantity;

                return true;
            }
        }

        public bool Delete(Guid cartId, string name)
        {
            lock (Lock)
            {
                if (!_inMemoryBasket.ContainsKey(cartId))
                {
                    return false;
                }

                var itemsCollection = _inMemoryBasket[cartId];

                if (itemsCollection.ContainsKey(name))
                {
                    itemsCollection.Remove(name);
                }

                return true;
            }
        }

        public Basket Read(Guid cartId)
        {
            lock (Lock)
            {
                if (!_inMemoryBasket.ContainsKey(cartId))
                {
                    return null;
                }

                return ToBasket(cartId);
            }
        }

        public Basket Read(Guid cartId, string name)
        {
            lock (Lock)
            {
                if (!_inMemoryBasket.ContainsKey(cartId) || !_inMemoryBasket[cartId].ContainsKey(name))
                {
                    return null;
                }

                return ToBasket(cartId, new List<string>() { name });
            }
        }

        public bool Update(Guid cartId, Item item)
        {
            lock (Lock)
            {
                if (!_inMemoryBasket.ContainsKey(cartId))
                {
                    return false;
                }

                var itemsCollection = _inMemoryBasket[cartId];

                if (!itemsCollection.ContainsKey(item.Name))
                {
                    itemsCollection.Add(item.Name, new Item { Name = item.Name });
                }

                itemsCollection[item.Name].Quantity = item.Quantity;

                return true;
            }
        }

        internal Basket ToBasket(Guid id, List<string> names = null)
        {
            if (!_inMemoryBasket.ContainsKey(id))
            {
                return null;
            }

            var output = new Basket()
            {
                Id = id,

                Items = _inMemoryBasket[id].Where(kvp => names == null || names.Contains(kvp.Value.Name))
                .Select(kvp => kvp.Value).ToList()
            };

            return output;
        }
    }

}
