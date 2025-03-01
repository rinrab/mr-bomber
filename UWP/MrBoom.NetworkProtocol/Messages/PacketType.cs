using System;
using System.Collections.Generic;
using System.Text;

namespace MrBoom.NetworkProtocol.Messages
{
    public enum PacketType
    {
        PlayerJoin = 1,
        ClientJoin = 2,
        LobbyInfo = 3,
        GameInfo = 4,
        ClientUpdate = 5,
    }
}
