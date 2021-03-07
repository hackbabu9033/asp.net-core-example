using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleHostServiceExample.MetricsInterface
{
    public interface IProcessorMetric
    {
        int GetUsage();
    }
}
