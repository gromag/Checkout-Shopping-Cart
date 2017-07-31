using System;
using System.Net.Http;
using Newtonsoft.Json;
using Checkout.Cart.Models;

namespace Checkout.Cart.Tests.Helpers
{
    public static class HttpResponseMessageExtensions
    {
        public static T ContentTo<T>(this HttpResponseMessage response) where T: class, new()
        {
            var t =  response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(t, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}
