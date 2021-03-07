using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHostServiceExample.MetricsInterface
{
    public interface IMetricDeliver
    {
        Task DeliverMetric(PerformanceMetric metricCounter);
    }
}
