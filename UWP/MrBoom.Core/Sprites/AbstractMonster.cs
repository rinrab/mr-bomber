// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Modules;
using MrBoom.Core.Sprites.Providers;
using MrBoom.Core.Sprites.Proxy;

namespace MrBoom.Core.Sprites
{
    public abstract class AbstractMonster : GameEntityBase, IServerGameEntity
    {
        public AbstractMonster(Map.MonsterData monsterData, int x, int y)
        {
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
            AddSingleton<SpriteDieSoundWorkerMonster>();

            AddSingleton<SpriteProxyProvider>();
        }
    }
}
