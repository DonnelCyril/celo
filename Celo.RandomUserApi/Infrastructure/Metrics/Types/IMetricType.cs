using System.Collections.Generic;

namespace Celo.RandomUserApi.Infrastructure.Metrics.Types
{
    /// <summary>
    /// Interface denoting the different metric types.
    /// </summary>
    public interface IMetricType
    {
        /// <summary>
        /// Gets the metric name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the help text for the metric.
        /// </summary>
        string HelpText { get; }

    }
}
