// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprite;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Proxy
{
    public class SpriteProxy : ISpriteProxy, IRemoteProxy
    {
        private GameSpriteInfo message;

        public virtual int X => message.X;
        public virtual int Y => message.Y;

        public int AnimateIndex => message.AnimateIndex;
        public int FrameIndex => message.FrameIndex;

        public Feature Features => 0;
        public SkullType? Skull => null;

        public int LifeCount => 1;

        public bool HasUnplugin => false;
        public bool HasSkull => false;

        public SpriteType Type => message.Type == GameSpriteType.Monster ? SpriteType.Monster : SpriteType.Player;
        public int SubType => message.SubType;

        public virtual void SetIncomingMessage(IMessage message)
        {
            this.message = (GameSpriteInfo)message;
        }

        public virtual IMessage GetOutcomingMessage()
        {
            return null;
        }

        public void ClientUpdate()
        {
        }
    }
}
