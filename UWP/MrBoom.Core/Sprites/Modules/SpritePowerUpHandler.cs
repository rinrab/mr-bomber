// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Service;
using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Modules
{
    public class SpritePowerUpHandler : IServerGameEntity
    {
        private readonly SpritePosition position;
        private readonly TerrainMap map;
        private readonly BomberServiceProvider services;

        public SpritePowerUpHandler(SpritePosition position,
                                    TerrainMap map,
                                    BomberServiceProvider services)
        {
            this.position = position;
            this.map = map;
            this.services = services;
        }

        public void ServerUpdate()
        {
            int cellX = (position.X + 8) / 16;
            int cellY = (position.Y + 8) / 16;
            Cell cell = map.GetCell(cellX, cellY);

            if (cell.Type == TerrainType.PowerUp)
            {
                var result = PickPowerUp(cell.PowerUpType);

                if (result == PowerUpPickResult.Pick)
                {
                    map.SetCell(cellX, cellY, new Cell(TerrainType.Free));
                    //PlaySound(SoundEffectType.Pick);
                }
                else if (result == PowerUpPickResult.Burn)
                {
                    map.BurnCell(cellX, cellY);
                }
            }
        }

        public PowerUpPickResult PickPowerUp(PowerUpType powerUpType)
        {
            foreach (IPowerUpHandler service in services.EnumerateServices<IPowerUpHandler>())
            {
                PowerUpPickResult result = service.PickPowerUp(powerUpType);

                if (result != PowerUpPickResult.Skip)
                {
                    return result;
                }
            }

            return PowerUpPickResult.Skip;
        }
    }
}
