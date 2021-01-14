using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using static DiCcontainer.ContainerBuilder;

namespace DiCcontainer
{
    public class ContainerBuilder
    {
        private readonly Container _container;
        public class ServiceScope : IServiceScope
        {
            /// <summary>
            /// set serviceProvider for this serviceScope
            /// </summary>
            /// <param name="serviceProvider"></param>
            public ServiceScope(IServiceProvider serviceProvider)
            {
                ServiceProvider = serviceProvider;
            }

            public IServiceProvider ServiceProvider { get; }

            public void Dispose()
            {
                (ServiceProvider as IDisposable)?.Dispose();
            }
        }

    }
}
