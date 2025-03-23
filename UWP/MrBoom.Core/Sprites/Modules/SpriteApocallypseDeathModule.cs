// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Modules
{
    public class SpriteApocalypseDeathModule : IServerGameEntity
    {
        private readonly SpritePosition position;
        private readonly SpriteHealthController healthController;
        private readonly ISoundController soundController;

        public SpriteApocalypseDeathModule(SpritePosition position,
                                           SpriteHealthController healthController,
                                           ISoundController soundController)
        {
            this.position = position;
            this.healthController = healthController;
            this.soundController = soundController;
        }

        public void ServerUpdate()
        {
            if (position.Cell.Type == TerrainType.Apocalypse)
            {
                healthController.Kill();
                soundController.PlaySound(SoundEffectType.PlayerDie);
            }
        }
    }
}
