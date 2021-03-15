using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleHostServiceExample.InstrumentEvaluator
{
    public interface ISampleDataIntegrator
    {
        /// <summary>
        /// caculate Average for generic Collection under specified operation and result
        /// </summary>
        /// <typeparam name="T1">type of data collection</typeparam>
        /// <typeparam name="T2">the type of Average</typeparam>
        /// <param name="dataList"></param>
        /// <param name="averageCaculator"></param>
        /// <returns></returns>
        public T2 CaculateDataAverage<T1, T2>(IEnumerable<T1> dataList, Func<T2, T1, T2> averageCaculator, T2 seed);

        //public decimal GetDataOverallState<T>(del)
    }
}
