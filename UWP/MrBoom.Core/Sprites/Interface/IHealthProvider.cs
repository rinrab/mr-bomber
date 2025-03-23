// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites.Interface
{
    public interface IHealthProvider
    {
        bool IsDie { get; }
        bool IsAlive { get; }

        int LifeCount { get; }
        bool HasUnplugin { get; }
    }
}
