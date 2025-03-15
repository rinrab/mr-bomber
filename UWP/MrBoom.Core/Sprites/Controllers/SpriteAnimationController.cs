// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class SpriteAnimationController : ISpriteAnimationProvider
    {
        // animation
        public int AnimateIndex { get; protected set; }
        public int FrameIndex { get; protected set; }

        public SpriteAnimationController()
        {
        }

        public void Animate()
        {
            FrameIndex++;
        }

        public void StopAnimation()
        {
            FrameIndex = 0;
        }

        public void SetAnimation(int animateIndex)
        {
            AnimateIndex = animateIndex;
            FrameIndex = 0;
        }
    }
}
