using System;
using System.Net.Http;
using System.Text;
using Xunit;
using FluentAssertions;
using System.Net;
using Newtonsoft.Json;
using Checkout.Cart.Models;
using Checkout.Cart.Tests.Helpers;
using System.Linq;

namespace Checkout.Cart.Tests
{
    public class CartTest: IClassFixture<TestFixture<Checkout.Cart.RestApi.Startup>>,  IDisposable
    {
        private HttpClient _httpClient;

        public CartTest(TestFixture<Checkout.Cart.RestApi.Startup> fixture)
        {
             _httpClient = fixture.Client;
        }

        public void Dispose() {}

        [Fact]
        public void ThatNewCartEndpointIsUp()
        {
            //Given
            var requestMessage = BuildDefaultRequest("/api/cart/new", HttpMethod.Post, null);
            //When
            var result = _httpClient.SendAsync(requestMessage).Result;
            //Then
            result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public void ThatNewCartEndpointRespondsWithANewEmptyCart()
        {
            //Given
            var requestMessage = BuildDefaultRequest("/api/cart/new", HttpMethod.Post, null);
            //When
            var result = _httpClient.SendAsync(requestMessage).Result;
            var basket = result.ContentTo<Basket>();
            //Then
            basket.Id.Should().NotBeEmpty();
            basket.Items.Should().HaveCount(0);
        }

        [Fact]
        public void ThatANewlyCreatedCartCanBeSubsequentlyRetrieved()
        {
            //Given
            Guid basketId = CreateANewBasket();

            //When
            var reqBasket = BuildDefaultRequest("/api/cart/" + basketId, HttpMethod.Get, null);
            var result = _httpClient.SendAsync(reqBasket).Result;
            var basket = result.ContentTo<Basket>();

            //Then
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            basket.Id.Should().Be(basketId);
            
        }

        [Fact]
        public void ThatANonExistingCartRespondWithANotFound()
        {
            //Given
            Guid fakeBasketId = Guid.NewGuid();

            //When
            var reqBasket = BuildDefaultRequest("/api/cart/" + fakeBasketId, HttpMethod.Get, null);
            var result = _httpClient.SendAsync(reqBasket).Result;

            //Then
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ThatAnItemCanBeAddedToTheBasketAndThenReturnedAsPartOfTheBasket()
        {
            //Given
            Guid basketId = CreateANewBasket();
            //When
            var item = new Item { Name = "Pepsi", Quantity = 1 };
            var reqNewItem = BuildDefaultRequest("/api/cart/" + basketId, HttpMethod.Post, item);
            var result = _httpClient.SendAsync(reqNewItem).Result;

            //Then 
            result.StatusCode.Should().Be(HttpStatusCode.Created);

            //And When
            var reqBasket = BuildDefaultRequest("/api/cart/" + basketId, HttpMethod.Get, null);
            var basketResult = _httpClient.SendAsync(reqBasket).Result;
            var basket = basketResult.ContentTo<Basket>();

            //Then 
            basket.Items.Should().HaveCount(1);
            basket.Items.First().ShouldBeEquivalentTo(item);
        }
        [Fact]
        public void ThatAnItemIfAddedMoreThanOnceItsQuantityIsUpdated()
        {
            //Given
            Guid basketId = CreateANewBasket();

            var name = "Pepsi";
            var quantity = 1;

            //When
            AddANewProduct(basketId, name, quantity);
            AddANewProduct(basketId, name, quantity);

            //Then
            Basket basket = GetBasket(basketId);

            basket.Items.Should().HaveCount(1);
            basket.Items.First().Quantity.Should().Be(quantity * 2);
        }

        [Fact]
        public void ThatTwoItemsCanBeAddedAndRespectiveQuantityIsCorrect()
        {
            //Given
            Guid basketId = CreateANewBasket();

            var prodName1 = "Pepsi";
            var prodQuantity1 = 1;

            //When
            AddANewProduct(basketId, prodName1, prodQuantity1);

            var prodName2 = "Fanta";
            var prodQuantity2 = 3;
            AddANewProduct(basketId, prodName2, prodQuantity2);

            //Then
            Basket basket = GetBasket(basketId);

            basket.Items.Should().HaveCount(2);
            basket.Items.First(i => i.Name == prodName1).Quantity.Should().Be(prodQuantity1);
            basket.Items.First(i => i.Name == prodName2).Quantity.Should().Be(prodQuantity2);
        }

        [Fact]
        public void ThatOneCanChangeQuantity()
        {
            //Given
            Guid basketId = CreateANewBasket();

            var product = "Pepsi";
            var prodQuantity1 = 3;

            //When
            AddANewProduct(basketId, product, prodQuantity1);

            var prodQuantity2 = 1;
            var response = UpdateProduct(basketId, product, prodQuantity2);
            Basket basket = GetBasket(basketId);

            //Then
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            basket.Items.Should().HaveCount(1);
            basket.Items.First().Quantity.Should().Be(prodQuantity2);
        }

        [Fact]
        public void ThatPutIsIdempotent()
        {
            //Given
            Guid basketId = CreateANewBasket();

            var product = "Pepsi";
            var prodQuantity1 = 3;

            //When

            UpdateProduct(basketId, product, prodQuantity1);
            UpdateProduct(basketId, product, prodQuantity1);
            UpdateProduct(basketId, product, prodQuantity1);

            //Then
            Basket basket = GetBasket(basketId);

            basket.Items.Should().HaveCount(1);
            basket.Items.First().Quantity.Should().Be(prodQuantity1);
        }

        [Fact]
        public void ThatNegativeQuantitiesAreNotAccepted()
        {
            //Given
            Guid basketId = CreateANewBasket();

            var product = "Pepsi";
            var prodQuantity1 = -1;

            //When
            var item = new Item { Name = product, Quantity = prodQuantity1 };
            var reqNewItem = BuildDefaultRequest("/api/cart/" + basketId, HttpMethod.Post, item);
            var result = _httpClient.SendAsync(reqNewItem).Result;

            //Then
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void ThatEmptyItemNamesAreNotAccepted()
        {
            //Given
            Guid basketId = CreateANewBasket();

            var product = "";
            var prodQuantity1 = 1;

            //When

            var item = new Item { Name = product, Quantity = prodQuantity1 };
            var reqNewItem = BuildDefaultRequest("/api/cart/" + basketId, HttpMethod.Post, item);
            var result = _httpClient.SendAsync(reqNewItem).Result;

            //Then
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public void ThatItemsCanBeDelete()
        {
            //Given
            Guid basketId = CreateANewBasket();

            var product = "Pepsi";
            var prodQuantity1 = 3;

            AddANewProduct(basketId, product, prodQuantity1);
            Basket basket = GetBasket(basketId);

            //When

            var item = new Item { Name = product };
            var reqNewItem = BuildDefaultRequest(String.Format("/api/cart/{0}/{1}", basketId, product), HttpMethod.Delete, null);
            var result = _httpClient.SendAsync(reqNewItem).Result;

            //Then
            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ThatAnItemCanBeAccessed()
        {
            //Given
            Guid basketId = CreateANewBasket();
            var item = new Item { Name = "Moretti", Quantity = 6 };

            AddANewProduct(basketId, item.Name, item.Quantity);
            AddANewProduct(basketId, "Fanta", 2);
            AddANewProduct(basketId, "Coke", 1);

            //When
            var reqBasket = BuildDefaultRequest(String.Format("/api/cart/{0}/{1}", basketId, item.Name), HttpMethod.Get, null);
            var basketResult = _httpClient.SendAsync(reqBasket).Result;
            var basket = basketResult.ContentTo<Basket>();

            //Then 
            basketResult.StatusCode.Should().Be(HttpStatusCode.OK);

            //Then 
            basket.Items.Should().HaveCount(1);
            basket.Items.First().ShouldBeEquivalentTo(item);
        }

        private Basket GetBasket(Guid basketId)
        {
            var reqBasket = BuildDefaultRequest("/api/cart/" + basketId, HttpMethod.Get, null);
            var basketResult = _httpClient.SendAsync(reqBasket).Result;
            var basket = basketResult.ContentTo<Basket>();
            return basket;
        }

        private HttpResponseMessage AddANewProduct(Guid basketId, string name, int quantity)
        {
            return AddOrUpdateANewProduct(basketId, name, quantity, HttpMethod.Post);
        }

        private HttpResponseMessage UpdateProduct(Guid basketId, string name, int quantity)
        {
           return AddOrUpdateANewProduct(basketId, name, quantity, HttpMethod.Put);
        }

        private HttpResponseMessage AddOrUpdateANewProduct(Guid basketId, string name, int quantity, HttpMethod method)
        {
            var item = new Item { Name = name, Quantity = quantity };
            var reqNewItem = BuildDefaultRequest("/api/cart/" + basketId, method, item);
            var result = _httpClient.SendAsync(reqNewItem).Result;

            return result;
        }

        private Guid CreateANewBasket()
        {
            var reqNewBasket = BuildDefaultRequest("/api/cart/new", HttpMethod.Post, null);
            var newBasketResult = _httpClient.SendAsync(reqNewBasket).Result;
            var basketId = newBasketResult.ContentTo<Basket>().Id;
            return basketId;
        }

        private HttpRequestMessage BuildDefaultRequest(string endpoint, HttpMethod method, object payLoad)
        {
            var requestMessage = new HttpRequestMessage(method, endpoint);
            var serialisedPayload = JsonConvert.SerializeObject(payLoad);
            var contentType = "application/json";

            requestMessage.Content = new StringContent(serialisedPayload, Encoding.UTF8, contentType);
            requestMessage.Headers.Add("Accept", contentType);

            return requestMessage;
        }
    }

}
