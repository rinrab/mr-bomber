// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Threading.Tasks;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;

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

        private IPlayerProxy proxy;

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

        public ServerPlayer InitializeServerPlayer(Terrain terrain, int team)
        {
            throw new NotImplementedException();
        }

        public ISpriteProxy InitializeProxy()
        {
            proxy = new PlayerProxy(Index);
            return proxy;
        }

        public IClientSprite InitializeClientSprite(ITerrainAccessor terrain, Assets assets)
        {
            return new ClientSpriteLocalHuman(terrain, proxy, assets, Controller);
        }
    }
}
