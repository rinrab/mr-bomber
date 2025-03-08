// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Diagnostics;
using MrBoom.Core.Sprite;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;

namespace MrBoom.Extensibility.Proxy
{
    public class SmoothProxy : ISpriteProxy, IClientGameEntity, IRemoteProxy
    {
        private readonly ISpriteProxy proxy;

        public SpriteType Type => proxy.Type;
        public int SubType => proxy.SubType;
        public Feature Features => proxy.Features;
        public SkullType? Skull => proxy.Skull;
        public int LifeCount => proxy.LifeCount;
        public bool HasUnplugin => proxy.HasUnplugin;
        public bool HasSkull => proxy.HasSkull;

        private int oldX;
        private int oldY;
        private int newX;
        private int newY;

        private int tick;

        public int X => Animate(oldX, newX, tick);
        public int Y => Animate(oldY, newY, tick);

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

        public int AnimateIndex => proxy.AnimateIndex;
        public int FrameIndex => proxy.FrameIndex;

        public SmoothProxy(ISpriteProxy proxy)
        {
            this.proxy = proxy;
        }

        public void ClientUpdate()
        {
            proxy.ClientUpdate();

            tick++;

            if (newX != proxy.X || newY != proxy.Y)
            {
                tick = 0;

                oldX = newX;
                oldY = newY;

                newX = proxy.X;
                newY = proxy.Y;
            }
        }

        public void SetIncomingMessage(IMessage message)
        {
            if (proxy is IRemoteProxy remoteProxy)
            {
                remoteProxy.SetIncomingMessage(message);
            }
        }

        public IMessage GetOutcomingMessage()
        {
            if (proxy is IRemoteProxy remoteProxy)
            {
                return remoteProxy.GetOutcomingMessage();
            }
            else
            {
                return null;
            }
        }
    }
}
