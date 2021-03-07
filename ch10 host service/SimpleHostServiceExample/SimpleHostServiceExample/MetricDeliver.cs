using SimpleHostServiceExample.MetricsInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHostServiceExample
{
    public class MetricDeliver : IMetricDeliver
    {
        public Task DeliverMetric(PerformanceMetric metricCounter)
        {
            Console.WriteLine($"[{DateTimeOffset.UtcNow}]{metricCounter}");
            return Task.CompletedTask;
        }
    }
}
