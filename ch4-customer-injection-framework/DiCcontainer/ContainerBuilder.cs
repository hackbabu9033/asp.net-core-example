﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using static DiCcontainer.ContainerBuilder;

namespace DiCcontainer
{
    public class ContainerBuilder
    {
        private readonly Container _container;

        public ContainerBuilder(Container container)
        {
            _container = container;
            _container.Register<IServiceScopeFactory>(c => new ServiceScopeFactory(container.CreateChild()), LifeCycle.Transient);
        }

        private class ServiceScopeFactory : IServiceScopeFactory
        {
            private readonly Container _container;
            public ServiceScopeFactory(Container container) => _container = container;

            public IServiceScope CreateScope()
            {
                return new ServiceScope(_container);
            }
        }

        public IServiceProvider BuildServiceProvider()
        {
            return _container;
        }

        public class ContainerProviderFactory : IServiceProviderFactory<ContainerBuilder>
        {
            public ContainerBuilder CreateBuilder(IServiceCollection services)
            {
                var container = new Container();
                foreach (var service in services)
                {
                    var lifeCycle = LifeCycleConvert.Convert(service.Lifetime);
                    if (service.ImplementationFactory != null)
                    {
                        container.Register(service.ImplementationFactory, lifeCycle);
                    }
                    else if (service.ImplementationInstance != null)
                    {
                        Func<Container,object> serviceFac = (container) => service.ImplementationInstance;
                        container.Register(serviceFac, lifeCycle);
                    }
                    else
                    {
                        container.Register(service.ServiceType,service.ImplementationType, lifeCycle);
                    }
                }
                return new ContainerBuilder(container) { };
            }

            public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
            {
                return containerBuilder.BuildServiceProvider();
            }
        }

        public ContainerBuilder Register(Assembly assembly)
        {
            _container.Register(assembly);
            return this;
        }


        private class ServiceScope : IServiceScope
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
