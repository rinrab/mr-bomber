// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Proxy
{
    public class ProxyHealthProvider : IHealthProvider, IServerGameEntity
    {
        private readonly ISpriteProxy proxy;
        private readonly SpriteAnimationController animation;

        public bool IsDie => proxy.IsDie;
        public bool IsAlive => !IsDie;

        public bool HasUnplugin => proxy.HasUnplugin;

        public ProxyHealthProvider(ISpriteProxy proxy,
                                   SpriteAnimationController animation)
        {
            this.proxy = proxy;
            this.animation = animation;
        }

        public void ServerUpdate()
        {
            if (IsDie)
            {
                animation.SetAnimation(4);
            }
        }
    }
}
