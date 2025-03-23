// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;

namespace MrBoom.Extensibility.Proxy
{
    public abstract class ProxyExtensionBase : ISpriteProxy, IClientGameEntity, IRemoteProxy
    {
        private readonly ISpriteProxy proxy;

        public virtual int X => proxy.X;
        public virtual int Y => proxy.Y;
        public virtual SpriteType Type => proxy.Type;
        public virtual int SubType => proxy.SubType;
        public virtual Feature Features => proxy.Features;
        public virtual SkullType? Skull => proxy.Skull;
        public virtual bool HasUnplugin => proxy.HasUnplugin;
        public virtual bool HasSkull => proxy.HasSkull;
        public virtual int AnimateIndex => proxy.AnimateIndex;
        public virtual int FrameIndex => proxy.FrameIndex;

        protected ProxyExtensionBase(ISpriteProxy proxy)
        {
            this.proxy = proxy;
        }

        public virtual void ClientUpdate()
        {
            proxy.ClientUpdate();
        }

        public virtual void SetIncomingMessage(IMessage message)
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
