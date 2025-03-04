// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Sprite;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public class ClientSpriteLocalHuman : Sprite, IClientSprite
    {
        private readonly IPlayerProxy proxy;
        private readonly IClientSprite client;
        public readonly IController Controller;

        public override bool HasUnplugin => proxy.HasUnplugin;
        public override bool HasSkull => proxy.HasSkull;

        public override SpriteType Type => proxy.Type;
        public override int SubType => proxy.SubType;

        public ClientSpriteLocalHuman(ITerrainAccessor terrain,
                                      IPlayerProxy proxy,
                                      Assets assets,
                                      IController controller) : base(terrain, proxy.X, proxy.Y, 3)
        {
            this.proxy = proxy;
            client = new ClientSprite(this, assets);
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

            if (Math.Abs(X - proxy.X) + Math.Abs(Y - proxy.Y) > 16)
            {
                MoveTo(proxy.X, proxy.Y);
            }

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
