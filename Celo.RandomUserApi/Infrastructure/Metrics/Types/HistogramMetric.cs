using System.Collections.Generic;

namespace Celo.RandomUserApi.Infrastructure.Metrics.Types
{
    /// <summary>
    /// <see cref="PrometheusMetricsService"/> for an explanation of different metric types.
    /// </summary>
    public class HistogramMetric : IMetricType
    {
        public HistogramMetric(string name, string helpText, double value, IEnumerable<double> buckets = null)
        {
            Name = name;
            HelpText = helpText;
            Value = value;
            Buckets = buckets;
        }

        public string Name { get; }
        public string HelpText { get; }
        public double Value { get; }

        public IEnumerable<double> Buckets { get; }
    }
}
