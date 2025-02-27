// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server
{
    public class LobbyServer : BackgroundService
    {
        private readonly IGameLobby lobby;
        private readonly IUdpServer udpServer;
        private readonly ILogger logger;

        public LobbyServer(IGameLobby lobby,
                           IUdpServer udpServer,
                           ILogger<LobbyServer> logger)
        {
            this.lobby = lobby;
            this.udpServer = udpServer;
            this.logger = logger;

            udpServer.OnPacketReceived += OnMessageReceived;
        }

        private void OnMessageReceived(Packet packet)
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
        }
    }
}
