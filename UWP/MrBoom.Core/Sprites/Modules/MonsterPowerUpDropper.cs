// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class MonsterPowerUpDropper : IBombDeathHandler
    {
        private readonly SpritePosition position;
        private readonly MrBoom.Terrain terrain;

        public MonsterPowerUpDropper(SpritePosition position,
                                     MrBoom.Terrain terrain)
        {
            this.position = position;
            this.terrain = terrain;
        }

        public void OnBombDied()
        {
            terrain.SetCell(position.CellX, position.CellY,
                            terrain.GeneratePowerUp(PowerUpType.Life));
        }
    }
}
