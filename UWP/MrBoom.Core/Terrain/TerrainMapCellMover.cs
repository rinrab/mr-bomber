// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Terrain
{
    public class TerrainMapCellMover : IServerGameEntity
    {
        private readonly TerrainMap map;
        private readonly TerrainMapBomber mapBomber;

        public TerrainMapCellMover(TerrainMap map, TerrainMapBomber mapBomber)
        {
            this.map = map;
            this.mapBomber = mapBomber;
        }

        public void ServerUpdate()
        {
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Cell cell = map[x, y];

                    if (cell.OffsetX == 0 && cell.OffsetY == 0)
                    {
                        var next = map[x + cell.DeltaX / 2, y + cell.DeltaY / 2];

                        bool isCollisionGoing = (cell.DeltaX != 0 && next.DeltaX != 0)
                                             || (cell.DeltaY != 0 && next.DeltaY != 0);

                        if (next.Type == TerrainType.Rubber)
                        {
                            cell.DeltaX = -cell.DeltaX;
                            cell.DeltaY = -cell.DeltaY;
                        }
                        else if (cell.Type == TerrainType.Bomb &&
                                 next.Type == TerrainType.Bomb
                                 && isCollisionGoing)
                        {
                            mapBomber.DetonateBomb(x, y);
                            continue;
                        }
                        else if (next.Type != TerrainType.Free)
                        {
                            cell.DeltaY = 0;
                            cell.DeltaX = 0;
                        }
                    }

                    int newX = (x * 16 + cell.OffsetX + cell.DeltaX + 8) / 16;
                    int newY = (y * 16 + cell.OffsetY + cell.DeltaY + 8) / 16;

                    if (newX != x || newY != y)
                    {
                        if (map[newX, newY].Type == TerrainType.Free)
                        {
                            map[x, y] = new Cell(TerrainType.Free);
                            map[newX, newY] = cell;

                            cell.OffsetX += (x - newX) * 16;
                            cell.OffsetY += (y - newY) * 16;
                        }
                        else
                        {
                            mapBomber.DetonateBomb(x, y);
                            continue;
                        }
                    }

                    cell.OffsetX += cell.DeltaX;
                    cell.OffsetY += cell.DeltaY;
                }
            }
        }
    }
}
