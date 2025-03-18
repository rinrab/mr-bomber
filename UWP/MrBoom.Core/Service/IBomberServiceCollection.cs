// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom.Core.Service
{
    public interface IBomberServiceProvider
    {
        object GetService(Type type);
        T GetService<T>();
    }
}
