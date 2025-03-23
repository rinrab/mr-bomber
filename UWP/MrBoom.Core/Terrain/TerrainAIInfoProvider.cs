// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites;
using MrBoom.Core.Sprites.Controllers;

namespace MrBoom.Core.Terrain
{
    public class TerrainAIInfoProvider : IServerGameEntity
    {
        private readonly Grid<bool> hasMonsterGrid;
        private readonly Grid<bool> isMonsterComingGrid;
        private readonly Grid<int> killablePlayerGrid;

        private readonly TerrainSpriteHost sprites;

        public TerrainAIInfoProvider(TerrainMap map, TerrainSpriteHost sprites)
        {
            hasMonsterGrid = new Grid<bool>(map.Width, map.Height, false);
            isMonsterComingGrid = new Grid<bool>(map.Width, map.Height, false);
            killablePlayerGrid = new Grid<int>(map.Width, map.Height, 0);

            this.sprites = sprites;
        }

        public void ServerUpdate()
        {
            hasMonsterGrid.Reset(false);
            isMonsterComingGrid.Reset(false);
            foreach (AbstractMonster m in sprites.GetMonsters())
            {
                var health = m.GetService<SpriteHealthController>();
                var monsterController = m.GetService<SpriteMovementController>();
                var position = m.GetService<SpritePosition>();

                if (health.IsAlive)
                {
                    hasMonsterGrid[position.CellX, position.CellY] = true;
                    isMonsterComingGrid[position.CellX + monsterController.Direction.DeltaX(),
                                        position.CellY + monsterController.Direction.DeltaY()] = true;
                }
            }

            killablePlayerGrid.Reset(0);
            foreach (ServerPlayer player in sprites.GetPlayers())
            {
                var health = player.GetService<SpriteHealthController>();
                var position = player.GetService<SpritePosition>();

                // TODO: Check for unplugin.
                killablePlayerGrid[position.CellX, position.CellY] |= (1 << player.Team);

                if (IsTouchingMonster(position.CellX, position.CellY))
                {
                    health.Damage();
                }
            }
        }

        public bool IsTouchingMonster(int cellX, int cellY)
        {
            return hasMonsterGrid[cellX, cellY];
        }

        public bool IsMonsterComing(int cellX, int cellY)
        {
            return isMonsterComingGrid[cellX, cellY];
        }

        public int GetKillablePlayers(int cellX, int cellY)
        {
            return killablePlayerGrid[cellX, cellY];
        }
    }
}
