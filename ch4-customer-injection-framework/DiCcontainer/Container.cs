﻿using DiCcontainer.CustomerAttribute;
using System;
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
            _service = new Dictionary<Type, object>();

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

        public Container Register(Type from,Type to,LifeCycle lifeCycle)
        {
            // 沒有提供service factory時，自行建立service的instance
            Func<LifeCycle, Type[], object> serviceFac = (_, args) => CreateServiceInstance(to);
            var registryService = new RegistryService(from, lifeCycle, serviceFac);
            if (_registryTable.TryGetValue(from, out RegistryService existedService))
            {
                _registryTable[from] = registryService;
                registryService.Next = existedService;
            }
            else
            {
                _registryTable.Add(from, registryService);
            }
            return this;
        }

        public Container Register<TService>(Func<Container,TService> service, LifeCycle lifeCycle = LifeCycle.Singleton)
        {   
            var registryType = typeof(TService);
            // 有service factory時，用提供的factory method去建立方法
            object serviceFac(LifeCycle _, Type[] args) => service(this);
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

        public Container Register(Type serviceType,Func<Container,object>service, LifeCycle lifeCycle = LifeCycle.Singleton)
        {
            var registryType = serviceType;
            Func<LifeCycle, Type[], object> serviceFac = (_, args) => service(this);
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

        public Container Register(Assembly assembly)
        {
            // get all type with MaptoAttribute from a specified assembly
            var typedAttributes = from type in assembly.GetExportedTypes()
                                 let maptoAttribute = type.GetCustomAttribute<MaptoAttribute>()
                                 where maptoAttribute != null
                                 select new { ServiceType = type, mappedServiceType = maptoAttribute.ServiceType,lifeCycle = maptoAttribute.LifeCycle};
            foreach (var attribute in typedAttributes)
            {
                this.Register(attribute.mappedServiceType, attribute.ServiceType, attribute.lifeCycle);
            }
            return this;
        }

        public Container CreateChild()
        {
            return new Container(this);
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
        public void Dispose()
        {
            _disposed = true;
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
            _service.Clear();
        }

        public object GetService(Type serviceType)
        {
            RegistryService registryService;
            Type[] genericArgs = null;
            if (!_registryTable.TryGetValue(serviceType, out registryService))
            {
                throw new ArgumentException($"instance of type {serviceType.Name} is not registed");
            }

            // if type is generic,get its type args
            if (serviceType.IsGenericType && !_registryTable.ContainsKey(serviceType))
            {
                genericArgs = serviceType.GetGenericArguments();
            }

            switch (registryService.LifeCycle)
            {
                case LifeCycle.Transient:
                    var service = registryService.ServiceFac(registryService.LifeCycle, genericArgs);
                    if (service is IDisposable disposable)
                    {
                        this._disposables.Add(disposable);
                    }
                    return service;
                case LifeCycle.Scope:
                    return GetOrCreateService(_service,serviceType, registryService, genericArgs,_disposables);
                case LifeCycle.Singleton:
                    return GetOrCreateService(_rootContainer._service, serviceType, registryService, genericArgs,_rootContainer._disposables);
            }
            return null;
        }

        private object GetOrCreateService(Dictionary<Type, object> service,Type serviceType,
            RegistryService registryService,Type[] genericArgs,List<IDisposable> disposables)
        {
            if (service.TryGetValue(serviceType, out var serviceInstance))
            {
                return serviceInstance;
            }
            serviceInstance = registryService.ServiceFac(registryService.LifeCycle, genericArgs);
            // if the service implement Idisposable , then add to _disposables list
            if (serviceInstance is IDisposable disposable)
            {
                disposables.Add(disposable);
            }
            service.Add(serviceType, serviceInstance);
            return serviceInstance;
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

        private object CreateServiceInstance(Type type)
        {
            var constructors = type.GetConstructors();
            if (constructors.Length == 0)
            {
                throw new NotImplementedException($"class {type.Name} has no GetConstructor");
            }
            var contructor = constructors.Where(c => c.IsPublic == true).FirstOrDefault();
            contructor ??= constructors.First();
            var parameters = contructor.GetParameters();
            var args = new object[parameters.Length];
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(type);
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
