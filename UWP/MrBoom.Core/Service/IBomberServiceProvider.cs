// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom.Core.Service
{
    public interface IBomberServiceCollection
    {
        void AddSingleton<T>(Func<ServiceProvider, T> implementationFactory) where T : class;
        void AddSingleton<T>(T instance) where T : class;
        void AddSingleton<T>() where T : class;
    }
}
