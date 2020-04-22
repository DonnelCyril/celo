using AutoMapper;
using Celo.RandomUserApi.Infrastructure.AutoMapper;
using Celo.RandomUserApi.Infrastructure.Cache;
using Celo.RandomUserApi.Infrastructure.Cdn;
using Celo.RandomUserApi.Infrastructure.CosmosDB;
using Celo.RandomUserApi.Infrastructure.Logging;
using Celo.RandomUserApi.Infrastructure.Metrics;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Celo.RandomUserApi.Infrastructure
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddAutoMapper((_, config) => { config.ShouldUseConstructor = ci => ci.IsPublic; }, typeof(Startup));
            services.AddSingleton<IDomainObjectMapper,DomainObjectMapper>();

            RegisterMediatRServices(services);
            services.AddSingleton<IUserStoreFactory, CosmosDBUserStoreFactory>();
            services.AddSingleton<ICdnCachePurge, FastlyCdnCachePurge>();
            services.AddSingleton<IMetricsService, PrometheusMetricsService>();
            return services;
        }

        private static void RegisterMediatRServices(IServiceCollection services)
        {
            services.AddMediatR(typeof(Startup));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>));
        }
    }
}
