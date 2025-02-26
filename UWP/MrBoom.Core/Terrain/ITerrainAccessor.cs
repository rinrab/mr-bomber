// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public interface ITerrainAccessor
    {
        int Width { get; }
        int Height { get; }

        Cell GetCell(int x, int y);
        bool IsWalkable(int x, int y);
    }
}
