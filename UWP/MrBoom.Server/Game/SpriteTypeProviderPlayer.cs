// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.Server.Lobby;

namespace MrBoom.Server.Game
{
    public class SpriteTypeProviderPlayer : ISpriteTypeProvider
    {
        private IClientInfo clientInfo;

        public SpriteTypeProviderPlayer(IClientInfo clientInfo)
        {
            this.clientInfo = clientInfo;
        }

        public GameSpriteType GetType(ClientInfo client)
        {
            if (clientInfo.Equals(client.CorishInfo))
            {
                return GameSpriteType.PlayerMe;
            }
            else
            {
                return GameSpriteType.Player;
            }
        }
    }
}
