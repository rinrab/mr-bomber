// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace MrBoom.Core.Terrain
{
    public interface ITerrainProxy : IClientGameEntity
    {
        int Width { get; }
        int Height { get; }

        int TimeLeft { get; }
        int ApocalypseSpeed { get; }
        int MaxApocalypse { get; }
        int LevelIndex { get; }

        IList<ISpriteProxy> Sprites { get; }

        Cell GetCell(int x, int y);
        bool IsWalkable(int x, int y);
    }
}
