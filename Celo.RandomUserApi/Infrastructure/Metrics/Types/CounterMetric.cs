namespace Celo.RandomUserApi.Infrastructure.Metrics.Types
{
    /// <summary>
    /// <see cref="PrometheusMetricsService"/> for an explanation of different metric types.
    /// </summary>
    public class CounterMetric : IMetricType
    {
        public CounterMetric(string name, string helpText, double value = 1)
        {
            Name = name;
            HelpText = helpText;
            Value = value;
        }

        public string Name { get; }

        public string HelpText { get; }

        public double Value { get; }
    }
}
