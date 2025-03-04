// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.NetworkProtocol.Proxy;

namespace MrBoom.Screens
{
    public class OnlineConnectionDiedScreen : IScreen
    {
        private readonly Assets assets;
        private int tick = 0;

        public OnlineConnectionDiedScreen(Assets assets)
        {
            this.assets = assets;
        }

        public void Update()
        {
            tick++;
        }

        public void Draw(SpriteBatch ctx)
        {
            assets.MrFond.Draw(ctx, 0, 0);

            var img = assets.UWU[tick / 30];
            img.Draw(ctx, 50, 200 / 2 - img.Height + 24);

            Game.DrawString(ctx, 100, 200 / 2 - 9, "lost connection to server", assets.Alpha[1]);
            Game.DrawString(ctx, 100, 200 / 2 + 1, "connection timed out", assets.Alpha[1]);
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
