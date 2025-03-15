// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public interface ISpriteAnimationProvider
    {
        int AnimateIndex { get; }
        int FrameIndex { get; }
    }
}
