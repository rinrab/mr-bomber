// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom.Core.Terrain
{
    public class TerrainInitialMapProvider
    {
        public Grid<Cell> Map { get; }
        public List<CellCoord> SpawnPoints { get; }

        public TerrainInitialMapProvider(Map mapData)
        {
            Map = new Grid<Cell>(mapData.Data[0].Length, mapData.Data.Length);
            SpawnPoints = new List<CellCoord>();

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    char src = mapData.Data[y][x];

                    string bonusStr = "123456789AB";
                    if (src == '#')
                    {
                        Map[x, y] = new Cell(TerrainType.PermanentWall);
                    }
                    else if (src == '-')
                    {
                        Map[x, y] = new Cell(TerrainType.TemporaryWall);
                    }
                    else if (src == '*')
                    {
                        SpawnPoints.Add(new CellCoord(x, y));
                        Map[x, y] = new Cell(TerrainType.Free);
                    }
                    else if (src == '%')
                    {
                        Map[x, y] = new Cell(TerrainType.Rubber);
                    }
                    else if (bonusStr.Contains(src.ToString()))
                    {
                        int index = bonusStr.IndexOf(src);
                        Map[x, y] = new Cell(TerrainType.PowerUp)
                        {
                            Index = 0,
                            animateDelay = 8,
                            PowerUpType = (PowerUpType)index
                        };
                    }
                    else
                    {
                        Map[x, y] = new Cell(TerrainType.Free);
                    }
                }
            }
        }
    }
}
