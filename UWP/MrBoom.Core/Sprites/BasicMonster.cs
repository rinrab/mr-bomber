// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;
using MrBoom.Core.Sprites;

namespace MrBoom
{
    public class BasicMonster : AbstractMonster
    {
        public BasicMonster(Terrain map, Map.BasicMonsterData monsterData,
                            IRandom random, int x, int y) : base(map, monsterData, x, y)
        {
            AddSingleton(monsterData);
            AddSingleton(random);
            AddSingleton<BasicMonsterController>();
        }
    }
}
