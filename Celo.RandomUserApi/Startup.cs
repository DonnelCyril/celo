using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Celo.RandomUserApi.Infrastructure;
using Celo.RandomUserApi.Infrastructure.ActionFilter;
using Celo.RandomUserApi.Infrastructure.ErrorHandling;
using CorrelationId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Celo.RandomUserApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddCorrelationId();
            services.AddInfrastructureServices();
            services.AddControllers(opts => { opts.Filters.Add(typeof(ResponseHeadersActionFilter)); }).AddNewtonsoftJson();
            services.AddSingleton<IConfigureOptions<ApiBehaviorOptions>, ConfigureApiBehaviorOptions>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCorrelationId();

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseGlobalExceptionHandler();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
