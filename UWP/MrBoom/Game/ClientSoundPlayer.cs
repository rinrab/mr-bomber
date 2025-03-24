// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public class ClientSoundPlayer : IServerGameEntity
    {
        private readonly ITerrainProxy proxy;
        private readonly SoundAssets soundAssets;

        public ClientSoundPlayer(ITerrainProxy proxy, SoundAssets soundAssets)
        {
            this.proxy = proxy;
            this.soundAssets = soundAssets;
        }

        public void ServerUpdate()
        {
            PlaySounds(proxy.GetAndResetSoundsToPlay());
        }

        private void PlaySounds(SoundEffectType soundsToPlay)
        {
            if (soundsToPlay.HasFlag(SoundEffectType.Bang)) soundAssets.Bang.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.PoseBomb)) soundAssets.PoseBomb.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Sac)) soundAssets.Sac.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Pick)) soundAssets.Pick.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.PlayerDie)) soundAssets.PlayerDie.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Oioi)) soundAssets.Oioi.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Ai)) soundAssets.Ai.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Addplayer)) soundAssets.Addplayer.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Victory)) soundAssets.Victory.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Draw)) soundAssets.Draw.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Clock)) soundAssets.Clock.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.TimeEnd)) soundAssets.TimeEnd.Play();
            if (soundsToPlay.HasFlag(SoundEffectType.Skull)) soundAssets.Skull.Play();
        }
    }
}
