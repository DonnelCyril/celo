using Celo.RandomUserApi.Infrastructure.Metrics.Types;

namespace Celo.RandomUserApi.Infrastructure.Metrics
{
    /// <summary>
    /// Defines a message that has a metric which needs to be observed.
    /// </summary>
    public interface IMetrics
    {
        public IMetricType Metric { get; }
    }
}