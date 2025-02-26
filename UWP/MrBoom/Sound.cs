// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace MrBoom
{
    public class SoundAssets
    {
        public class Sound
        {
            private readonly SoundEffect sound;

            public Sound(SoundEffect sound)
            {
                this.sound = sound;
            }

            public void Play()
            {
                sound.Play();
            }
        }

        public class Music
        {
            private readonly Song song;

            public Music(Song song)
            {
                this.song = song;
            }

            public void Play()
            {
                MediaPlayer.Play(song);
            }
        }

        public Sound Bang;
        public Sound PoseBomb;
        public Sound Sac;
        public Sound Pick;
        public Sound PlayerDie;
        public Sound Oioi;
        public Sound Ai;
        public Sound Addplayer;
        public Sound Addbot;
        public Sound Victory;
        public Sound Draw;
        public Sound Clock;
        public Sound TimeEnd;
        public Sound Skull;
        public Music[] Musics;

        public static SoundAssets Load(ContentManager content)
        {
            Sound loadSound(string name)
            {
                return new Sound(content.Load<SoundEffect>("sound\\" + name));
            }

            Music loadMusic(string name)
            {
                return new Music(content.Load<Song>("music\\" + name));
            }

            return new SoundAssets()
            {
                Bang = loadSound("bang"),
                PoseBomb = loadSound("posebomb"),
                Sac = loadSound("sac"),
                Pick = loadSound("pick"),
                PlayerDie = loadSound("player_die"),
                Oioi = loadSound("oioi"),
                Ai = loadSound("ai"),
                Addplayer = loadSound("addplayer"),
                Addbot = loadSound("addbot"),
                Victory = loadSound("victory"),
                Draw = loadSound("draw"),
                Clock = loadSound("clock"),
                TimeEnd = loadSound("time_end"),
                Skull = loadSound("skull"),
                Musics = new Music[]
                {
                    loadMusic("anar11"),
                    loadMusic("chipmunk"),
                    loadMusic("chiptune"),
                    loadMusic("deadfeel"),
                    loadMusic("drop"),
                    loadMusic("external"),
                    loadMusic("matkamie"),
                    loadMusic("unreeeal"),
                }
            };
        }
    }
}
