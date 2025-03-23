// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Modules
{
    public class SpriteMonsterDeathModule : IServerGameEntity
    {
        private readonly SpritePosition position;
        private readonly TerrainAIInfoProvider aiInfoProvider;
        private readonly SpriteHealthController healthController;

        public SpriteMonsterDeathModule(SpritePosition position,
                                        TerrainAIInfoProvider aiInfoProvider,
                                        SpriteHealthController healthController)
        {
            this.position = position;
            this.aiInfoProvider = aiInfoProvider;
            this.healthController = healthController;
        }

        public void ServerUpdate()
        {
            if (aiInfoProvider.IsTouchingMonster(position.CellX, position.CellY))
            {
                healthController.Damage();
            }
        }
    }
}
