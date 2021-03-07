using SimpleHostServiceExample.MetricsInterface;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleHostServiceExample
{
    public class MetricCollector : IGetNetworkThroughtput,
        IMemoryMetric,
        IProcessorMetric
    {
        public long GetMemoryUsage()
        {
            return PerformanceMetric.Create().Memory;
        }

        public long GetThroughtput()
        {
            return PerformanceMetric.Create().Network;
        }

        public int GetUsage()
        {
            return PerformanceMetric.Create().Processer;
        }
    }
}
