// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.Core.Sprites;

namespace MrBoom
{
    public class ClientTerrainSpriteHost : IServerGameEntity
    {
        public List<GameEntityBase> Sprites { get; }

        public ClientTerrainSpriteHost()
        {
            Sprites = new List<GameEntityBase>();
        }

        public void ServerUpdate()
        {
            foreach (var sprite in Sprites)
            {
                sprite.ServerUpdate();
            }
        }
    }
}
