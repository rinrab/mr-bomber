// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Terrain
{
    public class TerrainStartInfo
    {
        public int LevelIndex { get; }

        public TerrainStartInfo(int levelIndex)
        {
            LevelIndex = levelIndex;
        }
    }
}
