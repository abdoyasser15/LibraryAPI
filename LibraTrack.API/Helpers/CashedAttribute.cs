using Azure;
using Azure.Core;
using Library.Core.ServiceContract;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace LibraTrack.API.Helpers
{
    public class CashedAttribute : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ResposeCashService = context.HttpContext.RequestServices.GetService<IResponseCashService>();

            var cacheKey = GenerateCaheKey(context.HttpContext.Request);

            var Response = await ResposeCashService.GetCashResponseAsync(cacheKey);

            if (!string.IsNullOrEmpty(Response)) // Response Is Cashed
            {
                var result = new ContentResult
                {
                    Content = Response,
                    ContentType = "application/json",
                    StatusCode = 200 // OK
                };
                context.Result = result; // Set the result to the cached response
                return;
            }
            var executingActionContext = await next.Invoke();

            if(executingActionContext.Result is OkObjectResult okObjectResult &&okObjectResult.Value is not null)
            {
                // Cache the response
                await ResposeCashService.CashResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromMinutes(5));
            }
        }

        private string GenerateCaheKey(HttpRequest request)
        {
            var KeyBuilder = new StringBuilder();

            KeyBuilder.Append($"{request.Path}"); // /api/Books

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key)) // pageIndex=1&pageSize=10&sort=name
            {
                KeyBuilder.Append($"|{key}-{value}"); // /api/products|pageIndex-1|pageSize-10|sort-name
            }
            return KeyBuilder.ToString();
        }
    }
}
