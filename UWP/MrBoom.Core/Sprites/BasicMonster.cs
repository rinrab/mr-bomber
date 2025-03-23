// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites;

namespace MrBoom
{
    public class BasicMonster : AbstractMonster
    {
        public BasicMonster(Map.BasicMonsterData monsterData, int x, int y) : base(monsterData, x, y)
        {
            AddSingleton(monsterData);
            AddSingleton<BasicMonsterController>();
        }
    }
}
