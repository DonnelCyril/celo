using System.Linq;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.ExtensionMethods;
using CorrelationId;
using Microsoft.AspNetCore.Mvc;

namespace Celo.RandomUserApi.Controllers.ActionResult
{
    /// <summary>
    /// Creates a validation error response by inspecting the ModelState.
    /// </summary>
    public class ValidationProblemDetailsResult : IActionResult
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public ValidationProblemDetailsResult(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
            var errors = (from modelStateEntry in modelStateEntries
                          from modelStateError in modelStateEntry.Value.Errors
                          select new 
                          {
                              modelStateEntry.Key,
                              Errors = modelStateEntry.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                          })
                .ToDictionary(e => e.Key, e => e.Errors);
            var problemDetails = new ValidationProblemDetails(errors)
            {
                Status = 400,
                Title = "Request Validation Error",
                Instance = $"CorrelationId: {_correlationContextAccessor.CorrelationContext.CorrelationId}",
            };
            return context.HttpContext.Response.WriteJson(problemDetails);
        }
    }
}