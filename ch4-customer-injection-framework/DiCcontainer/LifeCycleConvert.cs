using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiCcontainer
{
    public static class LifeCycleConvert
    {
        public static ServiceLifetime Convert(LifeCycle lifeCycle)
        {
            switch (lifeCycle)
            {
                case LifeCycle.Transient:
                    return ServiceLifetime.Transient;
                case LifeCycle.Scope:
                    return ServiceLifetime.Scoped;
                case LifeCycle.Singleton:
                    return ServiceLifetime.Singleton;
                default:
                    return ServiceLifetime.Singleton;
            }
        }

        public static LifeCycle Convert(ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    return LifeCycle.Singleton;
                case ServiceLifetime.Scoped:
                    return LifeCycle.Scope;
                case ServiceLifetime.Transient:
                    return LifeCycle.Transient;
                default:
                    return LifeCycle.Transient;
            }
        }
    }
}
