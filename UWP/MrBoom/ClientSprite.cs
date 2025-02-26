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
        private readonly ISprite proxy;
        private readonly Assets.MovingSpriteAssets animations;

        public int X { get => proxy.X; }
        public int Y { get => proxy.Y; }

        public int AnimateIndex => proxy.AnimateIndex;
        public int frameIndex => proxy.frameIndex;

        public Feature Features => proxy.Features;
        public SkullType? Skull => proxy.Skull;

        public int LifeCount => proxy.LifeCount;

        public bool HasUnplugin => proxy.HasUnplugin;
        public bool HasSkull => proxy.HasSkull;

        private int blinking = 0;

        public ClientSprite(ISprite proxy, Assets.MovingSpriteAssets animations)
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

    public class ClientSpriteLocalHuman : MovableSprite, IClientSprite
    {
        private readonly IServerPlayer proxy;
        private readonly IClientSprite client;
        public readonly IController Controller;

        public ClientSpriteLocalHuman(ITerrainAccessor terrain, int x, int y,
                                      IServerPlayer proxy, Assets.MovingSpriteAssets animations,
                                      IController controller) : base(terrain, x, y)
        {
            this.proxy = proxy;
            client = new ClientSprite(proxy, animations);
            Controller = controller;
        }

        public void ClientUpdate()
        {
            if (Controller.IsKeyDown(PlayerKeys.Up))
            {
                proxy.SetDirection(Directions.Up);
            }
            else if (Controller.IsKeyDown(PlayerKeys.Left))
            {
                proxy.SetDirection(Directions.Left);
            }
            else if (Controller.IsKeyDown(PlayerKeys.Right))
            {
                proxy.SetDirection(Directions.Right);
            }
            else if (Controller.IsKeyDown(PlayerKeys.Down))
            {
                proxy.SetDirection(Directions.Down);
            }
            else
            {
                proxy.SetDirection(null);
            }

            if (Controller.IsKeyDown(PlayerKeys.Bomb))
            {
                proxy.ToggleDropBomb();
            }

            if (Controller.IsKeyDown(PlayerKeys.RcDitonate))
            {
                proxy.ToggleRemoteControl();
            }

            client.ClientUpdate();
        }

        public void Draw(SpriteBatch ctx)
        {
            client.Draw(ctx);
        }

        public override void KickBomb(int x, int y, int dx, int dy)
        {
            // nothing to kick yet
        }
    }
}
