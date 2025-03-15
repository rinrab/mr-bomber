// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprite;
using MrBoom.Core.Sprites;

namespace MrBoom
{
    public abstract class AbstractMonster : SpriteBase, IServerGameEntity
    {
        public AbstractMonster(Terrain terrain, Map.MonsterData monsterData,
                               int x, int y)
        {
            AddSingleton(terrain);
            AddSingleton(new SpriteStartInfo(x, y, monsterData.Speed, monsterData.LivesCount,
                                             SpriteType.Monster, monsterData.Type));

            AddSingleton<NullBombKicker>();
            AddSingleton<SpritePosition>();
            AddSingleton<SpriteAnimationController>();
            AddSingleton<SpriteHealthController>();
            AddSingleton<SpriteEffectController>();
            AddSingleton<SpriteSpeedProvider>();
            AddSingleton<SpriteMovementController>();

            AddSingleton<SpriteApocalypseDeathModule>();
            AddSingleton<SpriteBombDeathModule>();
            AddSingleton<MonsterPowerUpDropper>();

            AddSingleton<SpriteProxyProvider>();

            AddSingleton<MonsterController>();
        }
    }
}
