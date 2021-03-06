﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DiCcontainer
{
    public class Container : IServiceProvider,IDisposable
    {
        private Container _rootContainer;
        private Dictionary<Type, RegistryService> _registryTable;
        private Dictionary<Type, object> _service;
        private bool _disposed;
        private List<IDisposable> _disposables;

        public Container()
        {
            _rootContainer = this;
            _registryTable = new Dictionary<Type, RegistryService>();
            _service = new Dictionary<Type, object>();
            _disposed = false;
            _disposables = new List<IDisposable>();
        }

        public Container(Container parent)
        {
            _rootContainer = parent._rootContainer;
            _registryTable = _rootContainer._registryTable;
            _disposed = false;
            _disposables = new List<IDisposable>();
        }

        /// <summary>
        /// register service with given service instance
        /// </summary>
        /// <typeparam name="TFrom">the interface </typeparam>
        /// <typeparam name="Tto"></typeparam>
        /// <param name="service"></param>
        /// <param name="lifeCycle"></param>
        /// <returns></returns>
        public Container Register<TFrom, Tto>(LifeCycle lifeCycle = LifeCycle.Singleton)
            where Tto : TFrom
        {
            var registryType = typeof(TFrom);
            Func<LifeCycle, Type[], object> serviceFac = (_, args) => CreateServiceInstance<Tto>();
            var registryService = new RegistryService(registryType, lifeCycle, serviceFac);
            if (_registryTable.TryGetValue(registryType, out RegistryService existedService))
            {
                _registryTable[registryType] = registryService;
                registryService.Next = existedService;
            }
            else
            {
                _registryTable.Add(registryType, registryService);
            }
            return this;
        }

        /// <summary>
        /// register service with given service instance
        /// </summary>
        /// <typeparam name="TFrom">the interface </typeparam>
        /// <typeparam name="Tto"></typeparam>
        /// <param name="service"></param>
        /// <param name="lifeCycle"></param>
        /// <returns></returns>
        public Container Register<TFrom,Tto>(Tto service,LifeCycle lifeCycle = LifeCycle.Singleton)
            where Tto : TFrom
        {
            var registryType = typeof(TFrom);
            Func<LifeCycle, Type[], object> serviceFac = (_, args) => service;
            var registryService = new RegistryService(registryType, lifeCycle, serviceFac);
            if (_registryTable.TryGetValue(registryType, out RegistryService existedService))
            {
                _registryTable[registryType] = registryService;
                registryService.Next = existedService;
            }
            else
            {
                _registryTable.Add(registryType, registryService);
            }
            return this;
        }                

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            object serviceInstance = null;
            RegistryService registryService;
            Type[] genericArgs = null;
            if (!_registryTable.TryGetValue(serviceType, out registryService))
            {
                throw new ArgumentException("instance of type {0} is not registed");
            }

            // if type is generic,get its type args
            if (serviceType.IsGenericType && !_registryTable.ContainsKey(serviceType))
            {
                genericArgs = serviceType.GetGenericArguments();
            }

            switch (registryService.LifeCycle)
            {
                case LifeCycle.Transient:
                    return registryService.ServiceFac(registryService.LifeCycle, genericArgs);
                case LifeCycle.Scope:
                    _service.TryGetValue(serviceType, out serviceInstance);
                    if (serviceInstance == null)
                    {
                        serviceInstance = registryService.ServiceFac(registryService.LifeCycle, genericArgs);
                        _service.Add(serviceType, serviceInstance);
                        return serviceInstance;
                    }
                    _service[serviceType] = serviceInstance;
                    return serviceInstance;
                case LifeCycle.Singleton:
                    _rootContainer._service.TryGetValue(serviceType, out serviceInstance);
                    if (serviceInstance == null)
                    {
                        _service.Add(serviceType, serviceInstance);
                        serviceInstance = registryService.ServiceFac(registryService.LifeCycle, genericArgs);
                    }
                    _service[serviceType] = serviceInstance;
                    return serviceInstance;
            }
            return null;
        }

        private object CreateServiceInstance<T>()
        {
            var constructors = typeof(T).GetConstructors();
            if (constructors.Length == 0)
            {
                throw new NotImplementedException($"class {typeof(T).Name} has no GetConstructor");
            }
            var contructor = constructors.Where(c=>c.IsPublic == true).FirstOrDefault();
            contructor ??= constructors.First();
            var parameters = contructor.GetParameters();
            var args = new object[parameters.Length];
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(typeof(T));
            }
            for (int i = 0; i < parameters.Length; i++)
            {
                var a = parameters[i].ParameterType;
                var b = parameters[i].GetType();
                args[i] = GetService(parameters[i].ParameterType);
            }
            return contructor.Invoke(args);
        }
        
    }
}
