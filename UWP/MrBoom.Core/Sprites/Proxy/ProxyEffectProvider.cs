// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Proxy
{
    public class ProxyEffectProvider : IEffectProvider
    {
        private readonly ISpriteProxy proxy;

        public Feature Features => proxy.Features;
        public SkullType? Skull => proxy.Skull;
        public bool HasSkull => proxy.HasSkull;

        public ProxyEffectProvider(ISpriteProxy proxy)
        {
            this.proxy = proxy;
        }
    }
}
