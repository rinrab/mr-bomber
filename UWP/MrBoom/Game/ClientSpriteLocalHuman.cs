// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites;
using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Modules;
using MrBoom.Core.Sprites.Providers;
using MrBoom.Core.Sprites.Proxy;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public class ClientSpriteLocalHuman : GameEntityBase
    {
        public ClientSpriteLocalHuman(ITerrainProxy terrain,
                                      IPlayerProxy proxy,
                                      Assets assets,
                                      IController controller)
        {
            AddSingleton(terrain);
            AddSingleton(proxy);
            AddSingleton(assets);
            AddSingleton(controller);

            AddSingleton(new SpriteStartInfo(0, 0, 3, 1, proxy.Type, proxy.SubType));

            AddSingleton<SpritePosition>();
            AddSingleton<NullBombKicker>(); // TODO:
            AddSingleton<SpriteAnimationController>();
            AddSingleton<SpriteSpeedProvider>();
            AddSingleton<SpriteMovementController>();

            AddSingleton<ProxyAnimationProvider>();
            AddSingleton<ProxyEffectProvider>();
            AddSingleton<ProxyHealthProvider>();

            AddSingleton<ClientSpriteController>();
            AddSingleton<ClientSpriteGraphics>();
        }
    }
}
