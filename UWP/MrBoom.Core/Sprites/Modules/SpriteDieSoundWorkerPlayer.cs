// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Modules
{
    public class SpriteDieSoundWorkerPlayer : IDeathHandler
    {
        private readonly ISoundController soundController;

        public SpriteDieSoundWorkerPlayer(ISoundController soundController)
        {
            this.soundController = soundController;
        }

        public void OnDamaged()
        {
            soundController.PlaySound(SoundEffectType.Oioi);
        }

        public void OnDied(bool forced)
        {
            soundController.PlaySound(SoundEffectType.PlayerDie);
        }
    }
}
