// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites
{
    public class MonsterPowerUpDropper : IBombDeathHandler
    {
        private readonly SpritePosition position;
        private readonly TerrainMap map;
        private readonly PowerUpProvider powerUpProvider;

        public MonsterPowerUpDropper(SpritePosition position,
                                     TerrainMap map, PowerUpProvider powerUpProvider)
        {
            this.position = position;
            this.map = map;
            this.powerUpProvider = powerUpProvider;
        }

        public void OnBombDied()
        {
            map.SetCell(position.CellX, position.CellY,
                        powerUpProvider.GeneratePowerUp(PowerUpType.Life));
        }
    }
}
