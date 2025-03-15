// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.NetworkProtocol.Messages;
using MrBoom.Server.Lobby;

namespace MrBoom.Server.Game
{
    public class SpriteTypeProviderMonster : ISpriteTypeProvider
    {
        public GameSpriteType GetType(ClientInfo client)
        {
            return GameSpriteType.Monster;
        }
    }
}
