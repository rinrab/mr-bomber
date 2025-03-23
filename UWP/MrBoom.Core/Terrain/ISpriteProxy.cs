// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites;

namespace MrBoom.Core.Terrain
{
    public interface ISpriteProxy : ISprite, IClientGameEntity
    {
        SpriteType Type { get; }
        int SubType { get; }
    }
}
