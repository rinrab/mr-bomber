// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Proxy
{
    public class ProxyHealthProvider : IHealthProvider
    {
        private readonly ISpriteProxy proxy;

        public bool IsDie => throw new NotImplementedException();
        public bool IsAlive => !IsDie;

        public int LifeCount => proxy.LifeCount;
        public bool HasUnplugin => proxy.HasUnplugin;

        public ProxyHealthProvider(ISpriteProxy proxy)
        {
            this.proxy = proxy;
        }
    }
}
