// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class SpriteEffectController : IEffectProvider
    {
        // features
        public Feature Features { get; protected set; }
        public SkullType? Skull { get; protected set; }

        protected int skullTimer;

        public virtual bool HasSkull => skullTimer > 0;

        /// <summary>
        ///
        /// </summary>
        /// <param name="feature"></param>
        /// <returns>true if successfully picked the feature, false if the action cannot be done, for example, when we already had this feature</returns>
        public bool PickFeature(Feature feature)
        {
            if (Features.HasFlag(feature))
            {
                return false;
            }
            else
            {
                Features |= feature;
                return true;
            }
        }

        public void SetSkull(SkullType skullType)
        {
            skullTimer = 600;
            Skull = skullType;
        }
    }
}
