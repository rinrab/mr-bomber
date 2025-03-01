// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Proxy
{
    public class SpriteProxy : ISpriteProxy, IRemoteProxy
    {
        private GameSpriteInfo message;

        public virtual int X => message.X;
        public virtual int Y => message.Y;

        public int AnimateIndex => 0;
        public int FrameIndex => 0;

        public Feature Features => 0;
        public SkullType? Skull => null;

        public int LifeCount => 1;

        public bool HasUnplugin => false;
        public bool HasSkull => false;

        public virtual void SetIncomingMessage(IMessage message)
        {
            this.message = (GameSpriteInfo)message;
        }
    }
}
