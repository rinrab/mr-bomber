// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MrBoom.Core.Service;

namespace MrBoom.Core
{
    public class BomberServiceProvider : IBomberServiceProvider, IBomberServiceCollection
    {
        private class Service
        {
            public readonly Type type;
            public readonly Func<BomberServiceProvider, object> implementationFactory;
            public object instance;

            public Service(Type type, Func<BomberServiceProvider, object> implementationFactory)
            {
                this.type = type;
                this.implementationFactory = implementationFactory;
                instance = null;
            }

            public Service(Type type, object instance)
            {
                this.type = type;
                this.instance = instance;
            }

            public object GetService(BomberServiceProvider serviceProvider)
            {
                if (instance == null)
                {
                    instance = implementationFactory.Invoke(serviceProvider);
                }

                return instance;
            }

            public bool IsCompatibleFor(Type type)
            {
                return type.IsAssignableFrom(this.type);
            }
        }

        private List<Service> services;
        private List<IBomberServiceProvider> serviceProviders;

        public BomberServiceProvider()
        {
            services = new List<Service>();
            serviceProviders = new List<IBomberServiceProvider>();

            AddSingleton(this);
        }

        public void AddSingleton<T>(Func<BomberServiceProvider, T> implementationFactory) where T : class
        {
            services.Add(new Service(typeof(T), services => implementationFactory(services)));
        }

        public void AddSingleton<T>(T instance) where T : class
        {
            if (instance == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                services.Add(new Service(typeof(T), instance));
            }
        }

        private static object CreateInstance(Type type, IBomberServiceProvider services)
        {
            ConstructorInfo constructor = type.GetConstructors().First();
            ParameterInfo[] parameters = constructor.GetParameters();

            List<object> args = new List<object>(parameters.Length);

            foreach (ParameterInfo param in parameters)
            {
                object service = services.GetService(param.ParameterType);

                if (service != null)
                {
                    args.Add(service);
                }
                else
                {
                    throw new Exception($"Can't create instance of {type} because no service {param.ParameterType} can be provided.");
                }
            }

            return constructor.Invoke(args.ToArray());
        }

        public void AddSingleton<T>() where T : class
        {
            AddSingleton(services => (T)CreateInstance(typeof(T), services));
        }

        public IEnumerable<object> EnumerateServices(Type type)
        {
            foreach (Service service in services)
            {
                if (service.IsCompatibleFor(type))
                {
                    yield return service.GetService(this);
                }
            }
        }

        public IEnumerable<T> EnumerateServices<T>()
        {
            foreach (object service in EnumerateServices(typeof(T)))
            {
                yield return (T)service;
            }
        }

        public object GetService(Type type)
        {
            foreach (object service in EnumerateServices(type))
            {
                return service;
            }

            foreach (IBomberServiceProvider serviceProvider in serviceProviders)
            {
                object service = serviceProvider.GetService(type);

                if (service != null)
                {
                    return service;
                }
            }

            return null;
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }

        public void AddServiceProvider(IBomberServiceProvider serviceProvider)
        {
            serviceProviders.Add(serviceProvider);
        }
    }
}
