// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Modules
{
    public class MonsterPowerUpDropper : IDeathHandler
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

        public void OnDamaged()
        {
        }

        public void OnDied(bool forced)
        {
            if (!forced)
            {
                map.SetCell(position.CellX, position.CellY,
                            powerUpProvider.GeneratePowerUp(PowerUpType.Life));
            }
        }
    }
}
