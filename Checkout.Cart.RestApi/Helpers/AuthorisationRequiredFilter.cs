using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Checkout.Cart.RestApi.Helpers
{
    public class AuthorisationRequiredFilter : IActionFilter
    {
        private string _secret;
        public AuthorisationRequiredFilter(IConfiguration settings)
        {
             this._secret = settings.GetSection("AppSettings").GetSection("CheckoutSecretKey").Value;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var req = context.HttpContext.Request;
            var auth = req.Headers["Authorization"];
            if (auth != this._secret)
            {
                context.Result = new ContentResult()
                {
                    StatusCode = (int) HttpStatusCode.Unauthorized
                };
            }
        }

        //public void OnResourceExecuting(ResourceExecutingContext context)
        //{
        //    var req = context.HttpContext.Request;
        //    var auth = req.Headers["Authorization"];
        //    if (!String.IsNullOrEmpty(auth) && auth == this._secret)
        //    {
        //        context.Result = new ContentResult()
        //        {
        //            Content = "Resource unavailable - header should not be set"
        //        };
        //    }
        //}
    }
}
