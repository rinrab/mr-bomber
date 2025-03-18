// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MrBoom.Core
{
    public interface IBomberServiceProvider
    {
        object GetService(Type type);
        T GetService<T>();
    }

    public interface IBomberServiceCollection
    {
        void AddSingleton<T>(Func<ServiceProvider, T> implementationFactory) where T : class;
        void AddSingleton<T>(T instance) where T : class;
        void AddSingleton<T>() where T : class;
    }

    public class ServiceProvider : IBomberServiceProvider, IBomberServiceCollection
    {
        private class Service
        {
            public readonly Type type;
            public readonly Func<ServiceProvider, object> implementationFactory;
            public object instance;

            public Service(Type type, Func<ServiceProvider, object> implementationFactory)
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

            public object GetService(ServiceProvider serviceProvider)
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

        public ServiceProvider()
        {
            services = new List<Service>();
            AddSingleton(this);
        }

        public void AddSingleton<T>(Func<ServiceProvider, T> implementationFactory) where T : class
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
            return EnumerateServices(type).FirstOrDefault();
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
    }
}
