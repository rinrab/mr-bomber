// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Sprites.Providers;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.Server.Lobby;

namespace MrBoom.Server.Game
{
    public class SpriteUpdateBroadcaster
    {
        private readonly ISpritePositionProvider position;
        private readonly SpriteStartInfo startInfo;
        private readonly ISpriteAnimationProvider animation;
        private readonly ISpriteTypeProvider spriteTypeProvider;
        private readonly IHealthProvider health;

        public SpriteUpdateBroadcaster(ISpritePositionProvider position,
                                       SpriteStartInfo startInfo,
                                       ISpriteAnimationProvider animation,
                                       ISpriteTypeProvider spriteTypeProvider,
                                       IHealthProvider health)
        {
            this.position = position;
            this.startInfo = startInfo;
            this.animation = animation;
            this.spriteTypeProvider = spriteTypeProvider;
            this.health = health;
        }

        public GameSpriteInfo GetUpdateMessage(ClientInfo client)
        {
            return new GameSpriteInfo
            {
                X = position.X,
                Y = position.Y,
                Type = spriteTypeProvider.GetType(client),
                SubType = startInfo.SubType,
                AnimateIndex = animation.AnimateIndex,
                FrameIndex = animation.FrameIndex,
                IsDie = health.IsDie,
            };
        }
    }
}
