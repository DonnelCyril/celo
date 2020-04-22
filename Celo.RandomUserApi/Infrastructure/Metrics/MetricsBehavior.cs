using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Celo.RandomUserApi.Infrastructure.Metrics.Types;
using MediatR;

namespace Celo.RandomUserApi.Infrastructure.Metrics
{
    public class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IMetricsService _metricsService;

        public MetricsBehavior(IMetricsService metricsService)
        {
            _metricsService = metricsService;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var sw = new Stopwatch();
            sw.Start();
            var response = await next();
            if (request is IMetrics metric) _metricsService.ObserveMetric(metric.Metric);
            sw.Stop();
            var messageName = typeof(TRequest).Name;
            _metricsService.ObserveMetric(
                new HistogramMetric($"{messageName}_seconds", $"Duration (in seconds) for {messageName}", sw.ElapsedMilliseconds / 1000.0)
                );
            return response;
        }
    }
}