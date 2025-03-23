// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Terrain
{
    public class BasicSoundController : ISoundController
    {
        public SoundEffectType SoundsToPlay { get; protected set; }

        public void PlaySound(SoundEffectType sound)
        {
            SoundsToPlay |= sound;
        }

        public void ResetSounds()
        {
            SoundsToPlay = 0;
        }
    }
}
