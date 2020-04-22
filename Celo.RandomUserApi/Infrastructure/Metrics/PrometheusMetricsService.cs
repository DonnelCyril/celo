using System;
using Celo.RandomUserApi.Infrastructure.Metrics.Types;

namespace Celo.RandomUserApi.Infrastructure.Metrics
{
    public class PrometheusMetricsService : IMetricsService
    {
        // Full implementation not provided here due to time constraints.
        // There is a prometheus sdk for .net which can be used to record the different metrics.
        // It exposes a local endpoint in the app that the prometheus service then pings periodically to collect the metrics captured.
        // This is not a exhaustive list of different metric types.
        // I have listed two to show the intended usage and can be extended based on application needs.

        public void ObserveMetric(IMetricType metricType)
        {
            switch (metricType)
            {
                case CounterMetric counterMetric:
                     IncrementCounter(counterMetric);
                     break;
                case HistogramMetric histogramMetric:
                    ObserveHistogram(histogramMetric);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(metricType), "Unsupported metric type.");
            }
        }

        /// <summary>
        /// A counter is a cumulative metric that represents a single monotonically increasing
        /// counter whose value can only increase or be reset to zero on restart. For example, you
        /// can use a counter to represent the number of requests served, tasks completed, or errors.
        /// </summary>
        private void IncrementCounter(CounterMetric metric)
        {
            Console.WriteLine($"[CounterMetric]: {metric.Name}, Value: {metric.Value}");
        }

        /// <summary>
        /// A histogram samples observations (usually things like request durations or response
        /// sizes) and counts them in configurable buckets. It also provides a sum of all observed values.
        /// The list values observed values will aggregated against a configurable number of buckets
        ///  i.e. when buckets are 0, 1, 2, and the following
        /// values are observed: 1.7, 1.0, and 0.1, this will be recorded as:
        /// 0 - count=1
        /// 1 - count=2
        /// 2 - count=0
        /// </summary>
        private void ObserveHistogram(HistogramMetric metric)
        {
            Console.WriteLine($"[HistogramMetric]: {metric.Name}, Value: {metric.Value}");
        }

    }
}