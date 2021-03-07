using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SimpleHostServiceExample;
using SimpleHostServiceExample.MetricOption;
using SimpleHostServiceExample.MetricsInterface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHostServiceExample
{
    public class PerformanceMetric
    {
        private static readonly Random _random = new Random();

        public int Processer { set; get; }
        public long Memory { set; get; }
        public long Network { set; get; }

        public override string ToString()
        {
            return $"CPU：{Processer * 100}%; memory：{Memory / (1024 * 1024)}M";
        }

        public static PerformanceMetric Create()
        {
            return new PerformanceMetric()
            {
                Processer = _random.Next(1, 8),
                Memory = _random.Next(10, 100) * 1024 * 1024,
                Network = _random.Next(10, 100) * 1024 * 1024
            };
        }
    }
    public sealed class PerformanceMetricLogger : IHostedService
    {
        private readonly IProcessorMetric _processorMeteric;
        private readonly IMemoryMetric _memoryMeteric;
        private readonly IMetricDeliver _deliverMeteric;
        private readonly IGetNetworkThroughtput _networkThroughtputMeteric;
        private readonly TimeSpan _logInterval;
        private IDisposable _scheduler;

        public PerformanceMetricLogger(
            IProcessorMetric processorMeteric,
            IMemoryMetric memoryMeteric,
            IMetricDeliver deliverMeteric,
            IGetNetworkThroughtput networkThroughtputMeteric,
            IOptions<MetricCollectionOption> options)
        {
            _processorMeteric = processorMeteric;
            _memoryMeteric = memoryMeteric;
            _deliverMeteric = deliverMeteric;
            _networkThroughtputMeteric = networkThroughtputMeteric;
            _logInterval = options.Value.CaprtureInterval;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _scheduler = new Timer(logPerformance, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        private void logPerformance(object state)
        {
            //Console.WriteLine($"[{DateTimeOffset.Now}]{PerformanceMetric.Create()}");
            var metricCollector = new PerformanceMetric()
            {
                Processer = _processorMeteric.GetUsage(),
                Memory = _memoryMeteric.GetMemoryUsage(),
                Network = _networkThroughtputMeteric.GetThroughtput()
            };
            _deliverMeteric.DeliverMetric(metricCollector);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _scheduler?.Dispose();
            return Task.CompletedTask;
        }
    }
}

