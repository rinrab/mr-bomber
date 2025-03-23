// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Terrain
{
    public class TerrainMap : Grid<Cell>
    {
        private readonly ISoundController soundController;

        public TerrainMap(TerrainInitialMapProvider initialMap, ISoundController soundController)
            : base(initialMap.Map.Width, initialMap.Map.Height)
        {
            for (int i = 0; i < initialMap.Map.CellCount; i++)
            {
                this[i] = initialMap.Map[i];
            }

            this.soundController = soundController;
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

            soundController.PlaySound(SoundEffectType.Sac);
        }
    }
}
