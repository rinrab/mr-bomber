// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites
{
    public class SpriteMonsterDeathModule : IServerGameEntity
    {
        private readonly SpritePosition position;
        private readonly MrBoom.Terrain terrain;
        private readonly SpriteHealthController healthController;

        public SpriteMonsterDeathModule(SpritePosition position, MrBoom.Terrain terrain, SpriteHealthController healthController)
        {
            this.position = position;
            this.terrain = terrain;
            this.healthController = healthController;
        }

        public void ServerUpdate()
        {
            if (terrain.IsTouchingMonster(position.CellX, position.CellX))
            {
                healthController.Damage();
            }
        }
    }
}
