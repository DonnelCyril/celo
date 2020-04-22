using Celo.RandomUserApi.Controllers.ActionResult;
using CorrelationId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Celo.RandomUserApi.Infrastructure.ErrorHandling
{
    public class ConfigureApiBehaviorOptions : IConfigureOptions<ApiBehaviorOptions>
    {
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public ConfigureApiBehaviorOptions(ICorrelationContextAccessor correlationContextAccessor)
        {
            _correlationContextAccessor = correlationContextAccessor;
        }
        public void Configure(ApiBehaviorOptions options)
        {
            // The code below is to change the default model generation factory used to generate validation error response. 
            // This ensures that we have a standard model for returning errors and validations messages.
            // This also allows us to pass a correlationId in the response which can then be used to filter log messages.
            options.InvalidModelStateResponseFactory = ctx => new ValidationProblemDetailsResult(_correlationContextAccessor);
        }
    }
}