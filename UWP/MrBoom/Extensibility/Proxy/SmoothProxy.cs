// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Core.Terrain;

namespace MrBoom.Extensibility.Proxy
{
    public class SmoothProxy : ProxyExtensionBase
    {
        private int oldX;
        private int oldY;
        private int newX;
        private int newY;

        private int tick;

        public override int X => Animate(oldX, newX, tick);
        public override int Y => Animate(oldY, newY, tick);

        private static int Animate(int oldPos, int newPos, int tick)
        {
            int delta = newPos - oldPos;
            int direction = Math.Sign(delta);

            int rv = oldPos + direction * tick;

            if (Math.Abs(oldPos - rv) > Math.Abs(oldPos - newPos))
            {
                return newPos;
            }
            else
            {
                return rv;
            }
        }

        public SmoothProxy(ISpriteProxy proxy) : base(proxy)
        {
        }

        public override void ClientUpdate()
        {
            base.ClientUpdate();

            tick++;

            if (newX != base.X || newY != base.Y)
            {
                tick = 0;

                oldX = newX;
                oldY = newY;

                newX = base.X;
                newY = base.Y;
            }
        }
    }
}
