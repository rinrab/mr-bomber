// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Modules;
using MrBoom.Core.Sprites.Providers;
using MrBoom.Core.Sprites.Proxy;

namespace MrBoom.Core.Sprites
{
    public class ServerPlayer : GameEntityBase
    {
        public int Team;
        public int TeamMask { get => 1 << Team; }

        public IClientInfo ClientInfo { get; private set; }

        public ServerPlayer(int team, int index, IClientInfo clientInfo)
        {
            AddSingleton(new SpriteStartInfo(0, 0, 3, 1, SpriteType.Player, index));

            AddSingleton<NullBombKicker>();
            AddSingleton<SpritePosition>();
            AddSingleton<SpriteAnimationController>();
            AddSingleton<SpriteHealthController>();
            AddSingleton<SpriteEffectController>();
            AddSingleton<SpriteSpeedProvider>();
            AddSingleton<SpriteBombController>();
            AddSingleton<SpriteMovementController>();

            AddSingleton<SpritePowerUpHandler>();
            AddSingleton<TerrainPowerUpHandler>();

            AddSingleton<SpriteApocalypseDeathModule>();
            AddSingleton<SpriteMonsterDeathModule>();
            AddSingleton<SpriteBombDeathModule>();

            AddSingleton<SpriteProxyProvider>();
            AddSingleton<PlayerProxyProvider>();
            AddSingleton<SpriteDebugInfoProvider>();

            Team = team;
            ClientInfo = clientInfo;
        }

        public override void ServerUpdate()
        {
            base.ServerUpdate();

            //if (Skull == SkullType.Reverse)
            //{
            //    Direction = Direction.Reverse();
            //}
        }

        public void GiveAll()
        {
            SpriteEffectController effects = GetService<SpriteEffectController>();
            effects.PickFeature(Feature.RemoteControl);
            effects.PickFeature(Feature.Kick);
            effects.SetSkull(SkullType.Fast);
        }

        public virtual string GetCellDebugInfo(int cellX, int cellY)
        {
            return string.Empty;
        }

        public virtual string GetDebugInfo()
        {
            return string.Empty;
        }
    }
}
