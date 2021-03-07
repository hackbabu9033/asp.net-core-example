using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleHostServiceExample.MetricOption
{
    public class MetricCollectionOption
    {
        /// <summary>
        /// time span to console performance
        /// </summary>
        public TimeSpan CaprtureInterval { set; get; }
        /// <summary>
        /// specified portocal
        /// </summary>
        public TransportType Transport { set; get; }
        /// <summary>
        /// deliver Ip address
        /// </summary>
        public Endpoint DeliverTo { set; get; }
    }

    public enum TransportType
    {
        tcp,
        udp,
        http
    }

    public class Endpoint
    {
        public string Host { set; get; }

        public int Port { set; get; }

        public override string ToString()
        {
            return $"{Host}:{Port}";
        }
    }
}
