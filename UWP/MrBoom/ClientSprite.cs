// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Sprite;
using MrBoom.Core.Terrain;

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
        private readonly ISpriteProxy proxy;
        private readonly Assets.MovingSpriteAssets animations;

        public int X { get => proxy.X; }
        public int Y { get => proxy.Y; }

        public int AnimateIndex => proxy.AnimateIndex;
        public int FrameIndex => proxy.FrameIndex;

        public Feature Features => proxy.Features;
        public SkullType? Skull => proxy.Skull;

        public int LifeCount => proxy.LifeCount;

        public bool HasUnplugin => proxy.HasUnplugin;
        public bool HasSkull => proxy.HasSkull;

        private int blinking = 0;

        public ClientSprite(ISpriteProxy proxy, Assets assets)
        {
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

        public void ClientUpdate()
        {
            blinking++;
        }

        public void Draw(SpriteBatch ctx)
        {
            if (proxy.FrameIndex != -1)
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

                Image img = animation[proxy.FrameIndex / 20];

                int x = X + 8 + 8 - img.Width / 2;
                int y = Y + 16 - img.Height;

                if (proxy.AnimateIndex != 4 || proxy.FrameIndex / 20 < animations.Normal[4].Length)
                {
                    img.Draw(ctx, x, y, color);
                }
            }
        }
    }
}
