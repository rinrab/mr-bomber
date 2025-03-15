// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Sprite;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public class ClientSpriteGraphics : IServerGameEntity, IClientDrawableGameEntity
    {
        private readonly ISpritePositionProvider position;
        private readonly ISpriteAnimationProvider animationProvider;
        private readonly ISpriteProxy proxy;

        private int blinking = 0;
        private readonly Assets.MovingSpriteAssets animations;

        public ClientSpriteGraphics(ISpritePositionProvider position,
                                    ISpriteAnimationProvider animationProvider,
                                    ISpriteProxy proxy,
                                    Assets assets)
        {
            this.position = position;
            this.animationProvider = animationProvider;
            this.proxy = proxy;

            if (proxy.Type == SpriteType.Monster)
            {
                animations = assets.Monsters[proxy.SubType];
            }
            else
            {
                animations = assets.Players[proxy.SubType];
            }
        }

        public void ServerUpdate()
        {
            blinking++;
        }

        public void Draw(SpriteBatch ctx)
        {
            if (animationProvider.FrameIndex != -1)
            {
                Color color = Color.White;

                AnimatedImage animation = animations.Normal[animationProvider.AnimateIndex];
                if (proxy.HasUnplugin && blinking % 30 < 15)
                {
                    animation = animations.Ghost[animationProvider.AnimateIndex];
                }
                if (proxy.HasSkull && blinking % 30 > 15)
                {
                    animation = animations.Red[animationProvider.AnimateIndex];
                }

                Image img = animation[animationProvider.FrameIndex / 20];

                int x = position.X + 8 + 8 - img.Width / 2;
                int y = position.Y + 16 - img.Height;

                if (animationProvider.AnimateIndex != 4 || animationProvider.FrameIndex / 20 < animations.Normal[4].Length)
                {
                    img.Draw(ctx, x, y, color);
                }
            }
        }
    }
}
