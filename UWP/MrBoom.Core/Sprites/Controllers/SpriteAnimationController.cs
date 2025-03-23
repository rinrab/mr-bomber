// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class SpriteAnimationController : ISpriteAnimationProvider, IServerGameEntity
    {
        // animation
        public int AnimateIndex { get; protected set; }
        public int FrameIndex { get; protected set; }

        private bool isAnimating;

        public SpriteAnimationController()
        {
        }

        public void ServerUpdate()
        {
            if (isAnimating)
            {
                FrameIndex++;
            }
        }

        public void StopAnimation()
        {
            isAnimating = false;
            FrameIndex = 0;
        }

        public void SetAnimation(int animateIndex)
        {
            isAnimating = true;
            AnimateIndex = animateIndex;
        }
    }
}
