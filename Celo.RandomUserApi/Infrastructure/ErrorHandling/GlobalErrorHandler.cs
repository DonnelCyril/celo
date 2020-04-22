using System.Net;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.ExtensionMethods;
using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Celo.RandomUserApi.Infrastructure.ErrorHandling
{
    public static class GlobalErrorHandler
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                var logger = app.ApplicationServices.GetService(typeof(ILogger<object>)) as ILogger;
                var correlationContextAccessor = app.ApplicationServices.GetService(typeof(ICorrelationContextAccessor)) as ICorrelationContextAccessor;
                errorApp.Run(context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature == null) return Task.CompletedTask;
                    var correlationId = correlationContextAccessor?.CorrelationContext.CorrelationId;
                    logger.LogError("Unhandled exception. {0}", contextFeature.Error);
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Internal server error",
                        Detail = $"Use this id when reporting issues: {correlationId}",
                        Status = 500
                    };
                    return context.Response.WriteJson(problemDetails);
                });
            });
        }
    }
}