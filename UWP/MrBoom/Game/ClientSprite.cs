// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Sprites;
using MrBoom.Core.Sprites.Proxy;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public interface IClientDrawableGameEntity
    {
        void Draw(SpriteBatch ctx);
        void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale);
    }

    public class ClientSprite : GameEntityBase
    {
        public ClientSprite(ISpriteProxy proxy, Assets assets)
        {
            AddSingleton(proxy);
            AddSingleton(assets);

            AddSingleton<ProxyPositionProvider>();
            AddSingleton<ProxyAnimationProvider>();
            AddSingleton<ProxyEffectProvider>();
            AddSingleton<ProxyHealthProvider>();

            AddSingleton<ClientSpriteGraphics>();
        }
    }
}
