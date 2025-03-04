// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprite;

namespace MrBoom.Core.Terrain
{
    public interface ISpriteProxy : ISprite
    {
        SpriteType Type { get; }
        int SubType { get; }
    }
}
