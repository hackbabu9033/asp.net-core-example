using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleHostServiceExample.MetricsInterface;
using System;

namespace SimpleHostServiceExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var metricCollector = new MetricCollector();
            new HostBuilder()
                .ConfigureServices(service => service
                    .AddSingleton<IGetNetworkThroughtput>(metricCollector)
                    .AddSingleton<IMemoryMetric>(metricCollector)
                    .AddSingleton<IProcessorMetric>(metricCollector)
                    //inject the aboving service dependency for hostservice PerformanceMetricLogger
                    .AddSingleton<IMetricDeliver,MetricDeliver>()
                    .AddSingleton<IHostedService, PerformanceMetricLogger>())
                .Build()
                .Run();
        }
    }
}
