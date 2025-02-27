// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
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

        private void OnMessageReceived(Packet packet, IPEndPoint endPoint)
        {
            if (packet.Message is ClientJoin clientJoin)
            {
                lobby.ClientJoin(null, endPoint);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                var players = new List<LobbyPlayerInfo>();

                foreach (var player in lobby.GetPlayers())
                {
                    players.Add(new LobbyPlayerInfo
                    {
                        Id = player.Id,
                        Index = (byte)player.Index,
                        Name = player.Name
                    });
                }

                var msg = new Packet
                {
                    Message = new LobbyInfo
                    {
                        Players = players,
                    }
                };

                using var stream = new MemoryStream();
                using var writer = new BinaryWriter(stream);

                msg.WriteTo(writer);

                foreach (var client in lobby.GetClients())
                {
                    await udpServer.SendMessage(stream.ToArray(), client.IpAddress, stoppingToken);
                }

                await Task.Delay(100, stoppingToken);
            }
        }
    }
}
