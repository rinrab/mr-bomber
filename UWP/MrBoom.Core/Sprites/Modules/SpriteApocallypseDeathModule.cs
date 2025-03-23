// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;

namespace MrBoom.Core.Sprites.Modules
{
    public class SpriteApocalypseDeathModule : IServerGameEntity
    {
        private readonly SpritePosition position;
        private readonly SpriteHealthController healthController;

        public SpriteApocalypseDeathModule(SpritePosition position, SpriteHealthController healthController)
        {
            this.position = position;
            this.healthController = healthController;
        }

        public void ServerUpdate()
        {
            if (position.Cell.Type == TerrainType.Apocalypse)
            {
                healthController.Kill();
                //PlaySound(SoundEffectType.PlayerDie);
            }
        }
    }
}
