// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites.Interface
{
    public interface ISpriteAnimationProvider
    {
        int AnimateIndex { get; }
        int FrameIndex { get; }
    }
}
