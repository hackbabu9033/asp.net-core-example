using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace SimpleHostServiceExample.InstrumentEvaluator
{
    public class CaculationData : ISampleDataIntegrator
    {
        public T2 CaculateDataAverage<T1, T2>(IEnumerable<T1> dataList, Func<T2, T1, T2> averageCaculator, T2 seed)
        {
            var type = typeof(T2);
            var initial = seed ?? Activator.CreateInstance(type);

            dataList.Aggregate(seed, averageCaculator);

            return seed;
        }
    }
}
