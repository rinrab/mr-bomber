// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Proxy
{
    public class ProxyAnimationProvider : ISpriteAnimationProvider
    {
        private readonly ISpriteProxy proxy;

        public int AnimateIndex => proxy.AnimateIndex;
        public int FrameIndex => proxy.FrameIndex;

        public ProxyAnimationProvider(ISpriteProxy proxy)
        {
            this.proxy = proxy;
        }
    }
}
