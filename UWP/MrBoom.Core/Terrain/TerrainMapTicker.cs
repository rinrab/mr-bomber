// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Terrain
{
    public class TerrainMapTicker : IServerGameEntity
    {
        private readonly TerrainMap map;

        private int time;

        public TerrainMapTicker(TerrainMap map)
        {
            this.map = map;
        }

        public void ServerUpdate()
        {
            time++;

            for (int i = 0; i < map.CellCount; i++)
            {
                Cell cell = map[i];
                if (cell.Index != -1)
                {
                    int animateDelay = (cell.animateDelay <= 0) ? 6 : cell.animateDelay;
                    if (time % animateDelay == 0)
                    {
                        cell.Index++;
                        if (cell.Index >= cell.GetAnimationLength())
                        {
                            if (cell.Next == null)
                            {
                                cell.Index = 0;
                            }
                            else
                            {
                                map[i] = cell.Next;
                            }
                        }
                    }
                }
            }
        }
    }
}
