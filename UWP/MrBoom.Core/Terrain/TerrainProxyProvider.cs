// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Linq;

namespace MrBoom.Core.Terrain
{
    public class TerrainProxyProvider : ITerrainProxy
    {
        private readonly TerrainMap map;
        private readonly TerrainFinal final;
        private readonly TerrainSpriteHost sprites;
        private readonly TerrainTimer timer;
        private readonly TerrainStartInfo startInfo;
        private readonly BasicSoundController soundController;

        public int TimeLeft => timer.TimeLeft;
        public int Width => map.Width;
        public int Height => map.Height;
        public int LevelIndex => startInfo.LevelIndex;

        public int ApocalypseSpeed => timer.ApocalypseSpeed;
        public int MaxApocalypse => final.MaxApocalypse;

        public IList<ISpriteProxy> Sprites => sprites.GetSprites().Select(sprite => sprite.GetService<ISpriteProxy>()).ToList();

        public SoundEffectType SoundsToPlay => soundController.SoundsToPlay;

        public TerrainProxyProvider(TerrainMap map,
                                    TerrainFinal final,
                                    TerrainSpriteHost sprites,
                                    TerrainTimer timer,
                                    TerrainStartInfo startInfo,
                                    BasicSoundController soundController)
        {
            this.map = map;
            this.final = final;
            this.sprites = sprites;
            this.timer = timer;
            this.startInfo = startInfo;
            this.soundController = soundController;
        }

        public void ClientUpdate()
        {
        }

        public Cell GetCell(int x, int y)
        {
            return map[x, y];
        }

        public bool IsWalkable(int x, int y)
        {
            return map.IsWalkable(x, y);
        }
    }
}
