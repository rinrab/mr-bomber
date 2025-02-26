// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Extensions.Logging;
using MrBoom.Common;
using System.Net.Sockets;

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

            udpServer.OnMessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(UdpReceiveResult msg)
        {
            var packet = new Packet();
            packet.ReadFrom(new BinaryReader(new MemoryStream(msg.Buffer)));
            logger.LogInformation("Received message {Message}", packet.Message);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
        }
    }
}
