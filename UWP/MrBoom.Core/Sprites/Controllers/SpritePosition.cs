// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites
{
    public class SpritePosition : ISpritePositionProvider
    {
        private readonly ITerrainProxy terrain;

        // position
        public int X { get; set; }
        public int Y { get; set; }

        // current cell
        public int CellX => (X + 8) / 16;
        public int CellY => (Y + 8) / 16;
        public Cell Cell => terrain.GetCell(CellX, CellY);

        public SpritePosition(ITerrainProxy terrain, SpriteStartInfo startInfo)
        {
            this.terrain = terrain;
            X = startInfo.X;
            Y = startInfo.Y;
        }

        public void MoveTo(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
