// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites
{
    public class SpriteSpeedProvider : ISpeedProvider
    {
        private readonly int defaultSpeed;
        private readonly IEffectProvider effectProvider;
        private readonly SpriteAnimationController animationController;

        public SpriteSpeedProvider(SpriteStartInfo startInfo,
                                   IEffectProvider effectProvider,
                                   SpriteAnimationController animationController)
        {
            defaultSpeed = startInfo.DefaultSpeed;
            this.effectProvider = effectProvider;
            this.animationController = animationController;
        }

        public int ProvideActualSpeed()
        {
            if (effectProvider.Features.HasFlag(Feature.RollerSkates))
            {
                return 4;
            }
            else if (effectProvider.Skull == SkullType.Fast)
            {
                return 5;
            }
            else if (effectProvider.Skull == SkullType.Slow)
            {
                return 1;
            }
            else
            {
                return defaultSpeed;
            }
        }

        public int ProvideMovesCount(int speed)
        {
            if (speed == 1)
                return (animationController.FrameIndex % 3 == 0) ? 1 : 0;
            else if (speed == 2)
                return (animationController.FrameIndex % 2 == 0) ? 1 : 0;
            else if (speed == 3)
                return 1;
            else if (speed == 4)
                return 2;
            else if (speed == 5)
                return 4;
            else
                return 0;
        }

        public int ProvideMovesCount()
        {
            return ProvideMovesCount(ProvideActualSpeed());
        }
    }
}
