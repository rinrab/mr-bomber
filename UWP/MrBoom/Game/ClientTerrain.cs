// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public class ClientTerrain : GameEntityBase, IClientDrawableGameEntity
    {
        public ClientTerrain(ITerrainProxy proxy, Assets assets)
        {
            AddSingleton(proxy);

            AddSingleton(assets);
            AddSingleton(services => GetService<Assets>().Levels[proxy.LevelIndex]);
            AddSingleton(services => GetService<Assets>().Sounds);

            AddSingleton<ClientTerrainSpriteHost>();
            AddSingleton<TerrainGraphics>();
            AddSingleton<ClientSoundPlayer>();
        }

        public void Draw(SpriteBatch ctx)
        {
            foreach (var service in EnumerateServices<IClientDrawableGameEntity>())
            {
                service.Draw(ctx);
            }
        }
    }
}
