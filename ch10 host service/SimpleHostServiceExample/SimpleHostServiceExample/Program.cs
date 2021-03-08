using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleHostServiceExample.MetricOption;
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
                // setting host configuration to set enviroment
                .ConfigureHostConfiguration(cfgBuilder => cfgBuilder.AddCommandLine(args))
                //1. add json file to configuration build
                .ConfigureAppConfiguration((context, builder) => builder.AddJsonFile("appsettings.json", false)
                    .AddJsonFile(
                        $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
                        true))
                .ConfigureServices((conext, service) => service
                    .AddSingleton<IGetNetworkThroughtput>(metricCollector)
                    .AddSingleton<IMemoryMetric>(metricCollector)
                    .AddSingleton<IProcessorMetric>(metricCollector)
                    // inject the aboving service dependency for hostservice PerformanceMetricLogger
                    .AddSingleton<IMetricDeliver, MetricDeliver>()
                    .AddSingleton<IHostedService, PerformanceMetricLogger>()
                    .AddOptions()
                    //2. then we can use Configure method to bind Ioption and configuration file instance
                    .Configure<MetricCollectionOption>(conext.Configuration.GetSection("MetricCollection")))
                .Build()
                .Run();
        }
    }
}
