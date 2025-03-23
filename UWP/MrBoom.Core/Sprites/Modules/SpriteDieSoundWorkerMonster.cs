// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Modules
{
    public class SpriteDieSoundWorkerMonster : IDeathHandler
    {
        private readonly ISoundController soundController;

        public SpriteDieSoundWorkerMonster(ISoundController soundController)
        {
            this.soundController = soundController;
        }

        public void OnDamaged()
        {
            soundController.PlaySound(SoundEffectType.Ai);
        }

        public void OnDied(bool forced)
        {
            soundController.PlaySound(SoundEffectType.Ai);
        }
    }
}
