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
        public int FrameIndex => proxy.FrameIndex;

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

    public class ClientSpriteLocalHuman : Sprite, IClientSprite
    {
        private readonly IServerPlayer proxy;
        private readonly IClientSprite client;
        public readonly IController Controller;

        public override bool HasUnplugin => proxy.HasUnplugin;
        public override bool HasSkull => proxy.HasSkull;

        public ClientSpriteLocalHuman(ITerrainAccessor terrain, int x, int y,
                                      IServerPlayer proxy, Assets.MovingSpriteAssets animations,
                                      IController controller) : base(terrain, x, y, 3)
        {
            this.proxy = proxy;
            client = new ClientSprite(this, animations);
            Controller = controller;
        }

        private Directions? GetDirection()
        {
            if (Controller.IsKeyDown(PlayerKeys.Up))
            {
                return Directions.Up;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Left))
            {
                return Directions.Left;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Right))
            {
                return Directions.Right;
            }
            else if (Controller.IsKeyDown(PlayerKeys.Down))
            {
                return Directions.Down;
            }
            else
            {
                return null;
            }
        }

        public void ClientUpdate()
        {
            Direction = GetDirection();

            Features = proxy.Features;
            Skull = proxy.Skull;

            base.ServerUpdate();

            if (Controller.IsKeyDown(PlayerKeys.Bomb))
            {
                proxy.ToggleDropBomb();
            }

            if (Controller.IsKeyDown(PlayerKeys.RcDitonate))
            {
                proxy.ToggleRemoteControl();
            }

            proxy.MoveTo(X, Y);

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
