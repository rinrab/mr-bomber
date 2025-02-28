// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace MrBoom.Core.Terrain
{
    public interface ITerrainProxy : ITerrainAccessor
    {
        int TimeLeft { get; }
        int ApocalypseSpeed { get; }
        int MaxApocalypse { get; }
        int LevelIndex { get; }

        ICollection<ISpriteProxy> Sprites { get; }
    }
}
