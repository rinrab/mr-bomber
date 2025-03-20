// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;
using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites
{
    public class SpriteEffectController : IEffectProvider, IPowerUpHandler
    {
        private readonly IRandom random;

        // features
        public Feature Features { get; protected set; }
        public SkullType? Skull { get; protected set; }

        protected int skullTimer;

        public virtual bool HasSkull => skullTimer > 0;

        public SpriteEffectController(IRandom random)
        {
            this.random = random;
        }

        public PowerUpPickResult PickFeature(Feature feature)
        {
            if (Features.HasFlag(feature))
            {
                return PowerUpPickResult.Burn;
            }
            else
            {
                Features |= feature;
                return PowerUpPickResult.Pick;
            }
        }

        public PowerUpPickResult SetSkull(SkullType skullType)
        {
            skullTimer = 600;
            Skull = skullType;

            return PowerUpPickResult.Pick;
        }

        public PowerUpPickResult PickPowerUp(PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.RemoteControl)
            {
                return PickFeature(Feature.RemoteControl);
            }
            else if (powerUpType == PowerUpType.RollerSkate)
            {
                return PickFeature(Feature.RollerSkates);
            }
            else if (powerUpType == PowerUpType.Kick)
            {
                return PickFeature(Feature.Kick);
            }
            else if (powerUpType == PowerUpType.Skull)
            {
                return SetSkull(random.NextEnum<SkullType>());
            }
            else
            {
                return PowerUpPickResult.Skip;
            }
        }
    }
}
