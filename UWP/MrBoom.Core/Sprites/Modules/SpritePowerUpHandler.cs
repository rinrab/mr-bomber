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
        private readonly ISoundController soundController;

        public SpritePowerUpHandler(SpritePosition position,
                                    TerrainMap map,
                                    BomberServiceProvider services,
                                    ISoundController soundController)
        {
            this.position = position;
            this.map = map;
            this.services = services;
            this.soundController = soundController;
        }

        public void ServerUpdate()
        {
            if (position.Cell.Type == TerrainType.PowerUp)
            {
                var result = PickPowerUp(position.Cell.PowerUpType);

                if (result == PowerUpPickResult.Pick)
                {
                    map.SetCell(position.CellX, position.CellY, new Cell(TerrainType.Free));
                    soundController.PlaySound(SoundEffectType.Pick);
                }
                else if (result == PowerUpPickResult.Burn)
                {
                    map.BurnCell(position.CellX, position.CellY);
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
