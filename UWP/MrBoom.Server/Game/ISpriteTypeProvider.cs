// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.NetworkProtocol.Messages;
using MrBoom.Server.Lobby;

namespace MrBoom.Server.Game
{
    public interface ISpriteTypeProvider
    {
        GameSpriteType GetType(ClientInfo client);
    }
}