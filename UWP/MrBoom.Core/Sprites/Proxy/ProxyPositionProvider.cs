// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Proxy
{
    public class ProxyPositionProvider : ISpritePositionProvider
    {
        private readonly ISpriteProxy proxy;

        public int X => proxy.X;
        public int Y => proxy.Y;

        public ProxyPositionProvider(ISpriteProxy proxy)
        {
            this.proxy = proxy;
        }
    }
}
