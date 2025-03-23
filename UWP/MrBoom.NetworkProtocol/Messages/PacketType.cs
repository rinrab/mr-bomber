// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.NetworkProtocol.Messages
{
    public enum PacketType
    {
        PlayerJoin = 1,
        ClientJoin = 2,
        LobbyInfo = 3,
        GameInfo = 4,
        ClientUpdate = 5,
        Ping = 6,
    }
}
