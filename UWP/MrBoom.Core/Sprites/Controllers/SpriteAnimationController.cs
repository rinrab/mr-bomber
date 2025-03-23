// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites.Controllers
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
                if (AnimateIndex == 4) // die
                {
                    FrameIndex += 4;
                }
                else
                {
                    FrameIndex++;
                }
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

            if (AnimateIndex != animateIndex)
            {
                AnimateIndex = animateIndex;
                FrameIndex = 0;
            }
        }
    }
}
