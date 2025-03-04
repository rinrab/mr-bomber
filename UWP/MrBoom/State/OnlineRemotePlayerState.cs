// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.State
{
    public class OnlineRemotePlayerState : IPlayerState
    {
        private LobbyPlayerInfo info;

        public int Index { get => info.Index; }
        public string Name { get => info.Name; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        public OnlineRemotePlayerState(LobbyPlayerInfo info)
        {
            this.info = info;
        }

        public ServerPlayer GetPlayer(Terrain terrain, int team)
        {
            throw new NotImplementedException();
        }
    }
}
