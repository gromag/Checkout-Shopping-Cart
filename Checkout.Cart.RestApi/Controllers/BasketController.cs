using System;
using Microsoft.AspNetCore.Mvc;
using Checkout.Cart.Abstracts;
using Checkout.Cart.Models;
using System.Net;

namespace Checkout.Cart.RestApi.Controllers
{
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private ICartRepository _repository;

        public CartController(ICartRepository repository)
        {
            this._repository = repository;
        }

        [HttpPost("new")]
        public Basket New()
        {
            var output = this._repository.New();

            RespondWith(HttpStatusCode.Created);

            return output;

        }

        [HttpPost("{cartId}")]
        public void Post(Guid cartId, [FromBody]Item value)
        {
            if (!ModelState.IsValid)
            {
                RespondWith(HttpStatusCode.BadRequest);
                return;
            }
            var outcome = this._repository.Add(cartId, value);

            if (!outcome)
            {
                RespondWith(HttpStatusCode.NotFound);
            }
            else
            {
                RespondWith(HttpStatusCode.Created);
            }
        }

        [HttpPut("{cartId}")]
        public void Put(Guid cartId, [FromBody]Item value)
        {
            if (!ModelState.IsValid)
            {
                RespondWith(HttpStatusCode.BadRequest);
                return;
            }
            var outcome = this._repository.Update(cartId, value);

            if (!outcome)
            {
                RespondWith(HttpStatusCode.NotFound);
            }
        }

        [HttpGet("{cartId}")]
        public Basket Get(Guid cartId)
        {
            var basket = this._repository.Read(cartId);

            if(basket == null)
            {
                RespondWith(HttpStatusCode.NotFound);
            }

            return basket;
        }

        [HttpGet("{cartId}/{name}")]
        public Basket Get(Guid cartId,string name)
        {
            if (String.IsNullOrWhiteSpace(name))
            {
                RespondWith(HttpStatusCode.BadRequest);
            }

            var basket = this._repository.Read(cartId, name);

            if (basket == null)
            {
                RespondWith(HttpStatusCode.NotFound);
            }

            return basket;
        }

        [HttpDelete("{cartId}/{name}")]
        public void Delete(Guid cartId, string name)
        {
            this._repository.Delete(cartId, name);
        }

        private void RespondWith(HttpStatusCode code)
        {
            Response.StatusCode = (int)code;
        }

    }
}
