// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Service;
using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites.Modules
{
    public class SpriteBombDeathModule : IServerGameEntity
    {
        private readonly SpritePosition position;
        private readonly SpriteHealthController healthController;
        private readonly BomberServiceProvider serviceProvider;

        public SpriteBombDeathModule(SpritePosition position,
                                     SpriteHealthController healthController,
                                     BomberServiceProvider serviceProvider)
        {
            this.position = position;
            this.healthController = healthController;
            this.serviceProvider = serviceProvider;
        }

        public void ServerUpdate()
        {
            if (healthController.IsAlive)
            {
                if (position.Cell.Type == TerrainType.Fire && !healthController.HasUnplugin)
                {
                    healthController.Damage();
                }
            }
        }
    }
}
