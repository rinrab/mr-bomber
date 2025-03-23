// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Terrain
{
    public class TerrainMap : Grid<Cell>, IServerGameEntity
    {
        private readonly PowerUpProvider powerUpProvider;


        public TerrainMap(TerrainInitialMapProvider initialMap, PowerUpProvider powerUpProvider)
            : base(initialMap.Map.Width, initialMap.Map.Height)
        {
            this.powerUpProvider = powerUpProvider;

            for (int i = 0; i < initialMap.Map.CellCount; i++)
            {
                this[i] = initialMap.Map[i];
            }
        }

        public void ServerUpdate()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Cell cell = this[x, y];

                    if (cell.Type == TerrainType.Bomb)
                    {
                        if (!cell.rcAllowed || !cell.owner.IsAllowed)
                        {
                            cell.bombCountdown--;
                        }

                        if (cell.bombCountdown == 0 || (cell.owner != null && cell.owner.RemoteDetonate && cell.rcAllowed))
                        {
                            DetonateBomb(x, y);
                            continue;
                        }
                        if (cell.OffsetX == 0 && cell.OffsetY == 0)
                        {
                            var next = this[x + cell.DeltaX / 2, y + cell.DeltaY / 2];
                            if (next.Type == TerrainType.Rubber)
                            {
                                cell.DeltaX = -cell.DeltaX;
                                cell.DeltaY = -cell.DeltaY;
                            }
                            else if (next.Type == TerrainType.Bomb && ((cell.DeltaX != 0 && next.DeltaX != 0) || (cell.DeltaY != 0 && next.DeltaY != 0)))
                            {
                                DetonateBomb(x, y);
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
                            if (this[newX, newY].Type == TerrainType.Free)
                            {
                                this[x, y] = new Cell(TerrainType.Free);
                                this[newX, newY] = cell;

                                cell.OffsetX += (x - newX) * 16;
                                cell.OffsetY += (y - newY) * 16;
                            }
                            else
                            {
                                DetonateBomb(x, y);
                                continue;
                            }
                        }

                        cell.OffsetX += cell.DeltaX;
                        cell.OffsetY += cell.DeltaY;
                    }
                }
            }
        }

        public void DetonateBomb(int bombX, int bombY)
        {
            Cell bombCell = this[bombX, bombY];
            int maxBoom = bombCell.maxBoom;

            if (bombCell.owner != null)
            {
                bombCell.owner.OnBombReplaced(bombX, bombY);
            }

            void burn(int dx, int dy, FlameDirection middle, FlameDirection final)
            {
                for (int i = 1; i <= maxBoom; i++)
                {
                    int x = bombX + i * dx;
                    int y = bombY + i * dy;
                    Cell cell = this[x, y];

                    if (cell.Type == TerrainType.PermanentWall ||
                        cell.Type == TerrainType.Apocalypse ||
                        cell.Type == TerrainType.Rubber)
                    {
                        break;
                    };

                    if (cell.Type == TerrainType.TemporaryWall)
                    {
                        Cell next = powerUpProvider.GenerateGiven();

                        this[x, y] = new Cell(TerrainType.TemporaryWall)
                        {
                            Index = 0,
                            animateDelay = 4,
                            Next = next
                        };
                        break;
                    }
                    else if (cell.Type == TerrainType.PowerUp)
                    {
                        this[x, y] = new Cell(TerrainType.PowerUpFire)
                        {
                            Index = 0,
                            animateDelay = 6,
                            Next = new Cell(TerrainType.Free)
                        };
                        // PlaySound(SoundEffectType.Sac);
                        break;
                    }
                    else if (cell.Type == TerrainType.Bomb)
                    {
                        DetonateBomb(x, y);
                        break;
                    }
                    else if (cell.Type == TerrainType.Fire ||
                             cell.Type == TerrainType.PowerUpFire)
                    {
                    }
                    else
                    {
                        this[x, y] = new Cell(TerrainType.Fire)
                        {
                            FlameDirection = i == maxBoom ? final : middle,
                            Index = 0,
                            animateDelay = TerrainTimer.FLAME_ANIMATION_DELAY,
                            Next = new Cell(TerrainType.Free)
                        };
                    }
                }
            }

            // PlaySound(SoundEffectType.Bang);

            this[bombX, bombY] = new Cell(TerrainType.Fire)
            {
                Index = 0,
                animateDelay = TerrainTimer.FLAME_ANIMATION_DELAY,
                Next = new Cell(TerrainType.Free)
            };

            burn(1, 0, FlameDirection.BoomHor, FlameDirection.BoomRightEnd);
            burn(-1, 0, FlameDirection.BoomHor, FlameDirection.BoomLeftEnd);
            burn(0, 1, FlameDirection.BoomVert, FlameDirection.BoomBottomEnd);
            burn(0, -1, FlameDirection.BoomVert, FlameDirection.BoomTopEnd);
        }

        public void PutBomb(int cellX, int cellY, int maxBoom, bool rcAllowed, IBombOwner owner)
        {
            this[cellX, cellY] = new Cell(TerrainType.Bomb)
            {
                Index = 0,
                animateDelay = 12,
                bombCountdown = 210,
                maxBoom = maxBoom,
                rcAllowed = rcAllowed,
                owner = owner
            };
        }

        public void SetCell(int x, int y, Cell cell)
        {
            this[x, y] = cell;
        }

        public Cell GetCell(int x, int y)
        {
            return this[x, y];
        }

        public bool IsWalkable(int x, int y)
        {
            Cell cell = this[x, y];

            switch (cell.Type)
            {
                case TerrainType.Free:
                case TerrainType.PowerUpFire:
                    return true;

                case TerrainType.PermanentWall:
                case TerrainType.Rubber:
                case TerrainType.Apocalypse:
                    return false;

                case TerrainType.TemporaryWall:
                case TerrainType.Bomb:
                    return false; // cheats.noClip

                default:
                    return true;
            }
        }

        public void BurnCell(int cellX, int cellY)
        {
            SetCell(cellX, cellY, new Cell(TerrainType.PowerUpFire)
            {
                Index = 0,
                animateDelay = 6,
                Next = new Cell(TerrainType.Free)
            });

            // PlaySound(SoundEffectType.Sac);
        }
    }
}
