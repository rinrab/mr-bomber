// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Threading.Tasks;
using MrBoom.Bot;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom
{
    public interface IPlayerState
    {
        int Index { get; }
        string Name { get; }
        int VictoryCount { get; set; }
        bool IsReplaceble { get; }

        ServerPlayer GetPlayer(Terrain terrain, int team);
    }

    public interface IOnlinePlayerState : IPlayerState
    {
        Guid Id { get; }
    }

    public class HumanPlayerState : IPlayerState
    {
        public IController Controller { get; }
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => false;

        public HumanPlayerState(IController controller, int index, string name)
        {
            Controller = controller;
            Index = index;
            Name = name;
        }

        public ServerPlayer GetPlayer(Terrain terrain, int team)
        {
            return new ServerPlayer(terrain, team);
        }
    }

    public class OnlinePlayerState : IOnlinePlayerState
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

        public OnlinePlayerState(IController controller)
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

    public class BotPlayerState : IPlayerState
    {
        public int Index { get; }
        public string Name { get; }
        public int VictoryCount { get; set; }
        public bool IsReplaceble => true;

        public BotPlayerState(int index, string name)
        {
            Index = index;
            Name = name;
        }

        public ServerPlayer GetPlayer(Terrain terrain, int team)
        {
            return new ComputerPlayer(terrain, team, Index);
        }
    }
}
