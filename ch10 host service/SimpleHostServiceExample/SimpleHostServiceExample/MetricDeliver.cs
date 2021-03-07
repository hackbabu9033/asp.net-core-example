using Microsoft.Extensions.Options;
using SimpleHostServiceExample.MetricOption;
using SimpleHostServiceExample.MetricsInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHostServiceExample
{
    public class MetricDeliver : IMetricDeliver
    {
        private readonly TransportType _transportType;
        private readonly Endpoint _endpoint;

        public MetricDeliver(IOptions<MetricCollectionOption> options)
        {
            var optionContent = options.Value;
            _transportType = optionContent.Transport;
            _endpoint = optionContent.DeliverTo;
        }

        public Task DeliverMetric(PerformanceMetric metricCounter)
        {
            Console.WriteLine($"[{DateTimeOffset.UtcNow}] Deliver performance content " +
                $"{metricCounter} to {_endpoint} via {_transportType}");
            return Task.CompletedTask;
        }
    }
}
