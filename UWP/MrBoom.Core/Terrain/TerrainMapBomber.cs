// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Terrain
{
    public class TerrainMapBomber : IServerGameEntity
    {
        private readonly TerrainMap map;
        private readonly PowerUpProvider powerUpProvider;
        private readonly ISoundController soundController;

        public TerrainMapBomber(TerrainMap map, PowerUpProvider powerUpProvider, ISoundController soundController)
        {
            this.map = map;
            this.powerUpProvider = powerUpProvider;
            this.soundController = soundController;
        }

        public void ServerUpdate()
        {
            for (int i = 0; i < map.CellCount; i++)
            {
                Cell cell = map[i];

                if (cell.Type == TerrainType.Bomb)
                {
                    if (!cell.rcAllowed || !cell.owner.IsAllowed)
                    {
                        cell.bombCountdown--;
                    }

                    if (cell.bombCountdown == 0 || (cell.owner != null && cell.owner.RemoteDetonate && cell.rcAllowed))
                    {
                        DetonateBomb(map.GetCellX(i), map.GetCellY(i));
                        continue;
                    }
                }
            }
        }

        public void DetonateBomb(int bombX, int bombY)
        {
            Cell bombCell = map[bombX, bombY];
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
                    Cell cell = map[x, y];

                    if (cell.Type == TerrainType.PermanentWall ||
                        cell.Type == TerrainType.Apocalypse ||
                        cell.Type == TerrainType.Rubber)
                    {
                        break;
                    };

                    if (cell.Type == TerrainType.TemporaryWall)
                    {
                        Cell next = powerUpProvider.GenerateGiven();

                        map[x, y] = new Cell(TerrainType.TemporaryWall)
                        {
                            Index = 0,
                            animateDelay = 4,
                            Next = next
                        };
                        break;
                    }
                    else if (cell.Type == TerrainType.PowerUp)
                    {
                        map.BurnCell(x, y);
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
                        map[x, y] = new Cell(TerrainType.Fire)
                        {
                            FlameDirection = i == maxBoom ? final : middle,
                            Index = 0,
                            animateDelay = TerrainTimer.FLAME_ANIMATION_DELAY,
                            Next = new Cell(TerrainType.Free)
                        };
                    }
                }
            }

            soundController.PlaySound(SoundEffectType.Bang);

            map[bombX, bombY] = new Cell(TerrainType.Fire)
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
    }
}
