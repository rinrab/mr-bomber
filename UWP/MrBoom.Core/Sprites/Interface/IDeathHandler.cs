// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites.Interface
{
    public interface IDeathHandler
    {
        void OnDamaged();
        void OnDied(bool forced);
    }
}
