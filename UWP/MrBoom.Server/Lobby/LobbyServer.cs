// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;
using Haukcode.HighResolutionTimer;

namespace MrBoom.Server.Lobby
{
    public interface ILobbyState : IServerGameEntity
    {
        void OnPacketReceived(Packet packet, IPEndPoint endPoint);
        Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken);
    }

    public interface ILobbyStateManager
    {
        void SetState(ILobbyState state);
    }

    public class LobbyStateHolder : ILobbyState, ILobbyStateManager
    {
        private ILobbyState? current;

        public void SetState(ILobbyState state)
        {
            current = state;
        }

        public ILobbyState GetState()
        {
            return current;
        }

        public void OnPacketReceived(Packet packet, IPEndPoint endPoint)
        {
            current?.OnPacketReceived(packet, endPoint);
        }

        public void ServerUpdate()
        {
            current?.ServerUpdate();
        }

        public async Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken)
        {
            if (current != null)
            {
                await current.SendPackets(udpServer, stoppingToken);
            }
        }
    }

    public interface ILobbyProvider
    {
        Guid CreateLobby();
        Guid AssignLobby();
    }

    public class LobbyServer : TimerService, ILobbyProvider
    {
        private readonly IUdpServer udpServer;
        private readonly ILogger logger;

        private readonly Dictionary<Guid, LobbyStateHolder> lobbies;

        public LobbyServer(IUdpServer udpServer,
                           ILogger<LobbyServer> logger) : base(1000 / 20)
        {
            this.udpServer = udpServer;
            this.logger = logger;

            lobbies = new Dictionary<Guid, LobbyStateHolder>();

            udpServer.OnPacketReceived += OnMessageReceived;
        }

        public Guid CreateLobby()
        {
            Guid id = Guid.NewGuid();

            var state = new LobbyStateHolder();
            state.SetState(new LobbyJoinState(state, logger));

            lock (lobbies)
            {
                lobbies.Add(id, state);
            }

            logger.LogInformation("Created lobby {lobby}", id);

            return id;
        }

        public Guid AssignLobby()
        {
            lock (lobbies)
            {
                foreach (var lobby in lobbies)
                {
                    if (lobby.Value.GetState() is LobbyJoinState)
                    {
                        return lobby.Key;
                    }
                }

                return CreateLobby();
            }
        }

        private void OnMessageReceived(Packet packet, IPEndPoint endPoint)
        {
            lock (lobbies)
            {
                if (lobbies.TryGetValue(packet.Lobby, out var lobby))
                {
                    lobby.OnPacketReceived(packet, endPoint);
                }
                else
                {
                    logger.LogWarning("Rejected packet from {endPoint}, because lobby {lobby} doesn't exist.",
                                      endPoint, packet.Lobby);
                    // we are so sigmas for you
                }
            }
        }

        protected override async Task TickAsync(CancellationToken stoppingToken)
        {
            lock (lobbies)
            {
                foreach (LobbyStateHolder lobby in lobbies.Values)
                {
                    lobby.ServerUpdate();
                    lobby.ServerUpdate();
                    lobby.ServerUpdate();
                    _ = lobby.SendPackets(udpServer, stoppingToken);
                }
            }
        }
    }
}
