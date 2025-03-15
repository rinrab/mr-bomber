// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core;
using MrBoom.Core.Sprite;
using MrBoom.Core.Sprites;

namespace MrBoom
{
    public class ServerPlayer : SpriteBase
    {
        public int Team;
        public int TeamMask { get => 1 << Team; }

        public IClientInfo ClientInfo { get; private set; }

        protected readonly Terrain terrain;

        public ServerPlayer(Terrain terrain, int team, int index, IClientInfo clientInfo)
        {
            AddSingleton(terrain);
            AddSingleton(terrain.Random);
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

            AddSingleton<SpriteApocalypseDeathModule>();
            AddSingleton<SpriteMonsterDeathModule>();
            AddSingleton<SpriteBombDeathModule>();

            AddSingleton<SpriteProxyProvider>();

            Team = team;
            this.terrain = terrain;
            ClientInfo = clientInfo;
        }

        public override void ServerUpdate()
        {
            base.ServerUpdate();

            if (GetService<SpriteHealthController>().IsDie)
            {
                GetService<SpriteAnimationController>().Animate();
                return;
            }

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
