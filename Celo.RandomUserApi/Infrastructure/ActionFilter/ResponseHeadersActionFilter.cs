using Celo.RandomUserApi.Infrastructure.ActionFilter.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Celo.RandomUserApi.Infrastructure.ActionFilter
{

    /// <summary>
    /// Inspects the ActionResult and add any header information to the HTTP response header.
    /// </summary>
    public class ResponseHeadersActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            context.Result = (context.Result as ObjectResult)?.Value is ResponseWrapper responseWrapper
                ? CreateResponse(context, responseWrapper)
                : context.Result;
        }

        private static IActionResult CreateResponse(ActionContext context, ResponseWrapper responseWrapper)
        {
            var (status, headers, body) = responseWrapper;
            UpdateHeaders();
            var result = body != null
                ? new ObjectResult(body) { StatusCode = (int)status }
                : new StatusCodeResult((int)status) as ActionResult;
            return result;

            void UpdateHeaders()
            {
                var httpContext = context.HttpContext;
                foreach (var (key, value) in headers)
                {
                    if (httpContext.Response.Headers.ContainsKey(key))
                        httpContext.Response.Headers[key] = value;
                    else
                        httpContext.Response.Headers.Add(key, value);
                }
            }
        }
    }
}
