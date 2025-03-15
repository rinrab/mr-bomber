// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public interface IEffectProvider
    {
        Feature Features { get; }
        SkullType? Skull { get; }
        bool HasSkull { get; }
    }
}
