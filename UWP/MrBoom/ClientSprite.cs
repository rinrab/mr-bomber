// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public interface IClientGameEntity
    {
        void ClientUpdate();
    }

    public interface IClientDrawableGameEntity
    {
        void Draw(SpriteBatch ctx);
    }

    public interface IClientSprite : IClientGameEntity, IClientDrawableGameEntity, ISprite
    {
    }

    public class ClientSprite : IClientSprite
    {
        private readonly Sprite proxy;
        private readonly Assets.MovingSpriteAssets animations;

        public int X { get => proxy.X; }
        public int Y { get => proxy.Y; }

        private int blinking = 0;

        public ClientSprite(Sprite proxy, Assets.MovingSpriteAssets animations)
        {
            this.proxy = proxy;
            this.animations = animations;
        }

        public void ClientUpdate()
        {
            blinking++;
        }

        public void Draw(SpriteBatch ctx)
        {
            if (proxy.frameIndex != -1)
            {
                Color color = Color.White;

                AnimatedImage animation = animations.Normal[proxy.AnimateIndex];
                if (proxy.HasUnplugin && blinking % 30 < 15)
                {
                    animation = animations.Ghost[proxy.AnimateIndex];
                }
                if (proxy.HasSkull && blinking % 30 > 15)
                {
                    animation = animations.Red[proxy.AnimateIndex];
                }

                Image img = animation[proxy.frameIndex / 20];

                int x = X + 8 + 8 - img.Width / 2;
                int y = Y + 16 - img.Height;

                if (proxy.AnimateIndex != 4 || proxy.frameIndex / 20 < animations.Normal[4].Length)
                {
                    img.Draw(ctx, x, y, color);
                }
            }
        }
    }
}
