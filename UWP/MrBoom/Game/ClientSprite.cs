// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Sprites;
using MrBoom.Core.Sprites.Proxy;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public interface IClientDrawableGameEntity
    {
        void Draw(SpriteBatch ctx);
    }

    public class ClientSprite : GameEntityBase
    {
        public ClientSprite(ISpriteProxy proxy, Assets assets)
        {
            AddSingleton(proxy);
            AddSingleton(assets);

            AddSingleton<ProxyPositionProvider>();
            AddSingleton<ProxyAnimationProvider>();
            AddSingleton<ProxyHealthProvider>();

            AddSingleton<ClientSpriteGraphics>();
        }
    }
}
