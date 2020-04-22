using Celo.RandomUserApi.Infrastructure.Metrics.Types;

namespace Celo.RandomUserApi.Infrastructure.Metrics
{
    /// <summary>
    /// Used to capture application metrics.
    /// </summary>
    public interface IMetricsService
    {
        void ObserveMetric(IMetricType metricType);
    }
}