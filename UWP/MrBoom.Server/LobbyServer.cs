// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server
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

    public class LobbyServer : BackgroundService
    {
        private readonly IUdpServer udpServer;
        private readonly ILogger logger;

        private readonly LobbyStateHolder state;

        public LobbyServer(IUdpServer udpServer,
                           ILogger<LobbyServer> logger)
        {
            this.udpServer = udpServer;
            this.logger = logger;
            state = new LobbyStateHolder();

            state.SetState(new LobbyJoinState(state, logger));

            udpServer.OnPacketReceived += OnMessageReceived;
        }

        private void OnMessageReceived(Packet packet, IPEndPoint endPoint)
        {
            state.OnPacketReceived(packet, endPoint);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                state.ServerUpdate();
                await state.SendPackets(udpServer, stoppingToken);

                await Task.Delay(1000 / 60, stoppingToken);
            }
        }
    }
}
