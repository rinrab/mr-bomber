// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Threading.Tasks;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.State
{
    public class OnlineLocalPlayerState : IPlayerState
    {
        public enum SyncState
        {
            ClientInitialized,
            ServerRequested,
            ServerApproved,
            ServerRejected,
        }

        public SyncState State = SyncState.ClientInitialized;

        public IController Controller { get; }
        public int Index { get; private set; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        public string Name { get; private set; }

        public Guid Id { get; }

        public OnlineLocalPlayerState(IController controller)
        {
            Controller = controller;
            Index = -1;
            Id = Guid.NewGuid();
            Name = "...";
        }

        public void OnLoaded(LobbyPlayerInfo info)
        {
            Name = info.Name;
            State = SyncState.ServerApproved;
            Index = info.Index;
        }

        public async Task RequestServer(MultiplayerClient multiplayerClient)
        {
            State = SyncState.ServerRequested;

            await multiplayerClient.SendPacket(new Packet
            {
                Lobby = multiplayerClient.LobbyId,
                ClientSecret = multiplayerClient.ClientSecret,
                Message = new PlayerJoin
                {
                    Id = Id,
                }
            });
        }

        public ServerPlayer GetPlayer(Terrain terrain, int team)
        {
            throw new NotImplementedException();
        }
    }
}
