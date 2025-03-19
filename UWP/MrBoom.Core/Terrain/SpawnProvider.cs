// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.Common;

namespace MrBoom.Core.Terrain
{
    public class SpawnProvider
    {
        private readonly List<CellCoord> spawns;
        private readonly IRandom random;

        public SpawnProvider(IRandom random, TerrainInitialMapProvider initialMap)
        {
            spawns = new List<CellCoord>(initialMap.SpawnPoints);

            random.Shuffle(spawns);

            this.random = random;
        }

        public CellCoord? GenerateSpawn()
        {
            if (spawns.Count <= 0)
            {
                return null;
            }

            int spawnIndex = random.Next(spawns.Count);

            var spawn = spawns[spawnIndex];
            spawns.RemoveAt(spawnIndex);
            return spawn;
        }
    }
}
