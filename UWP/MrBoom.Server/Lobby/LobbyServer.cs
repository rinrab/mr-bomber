// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;
using Haukcode.HighResolutionTimer;

namespace MrBoom.Server.Lobby
{
    public class LobbyServer : TimerService, ILobbyProvider
    {
        private readonly IUdpServer udpServer;
        private readonly ILogger logger;

        private readonly Dictionary<Guid, ILobby> lobbies;

        public LobbyServer(IUdpServer udpServer,
                           ILogger<LobbyServer> logger) : base(1000 / 20)
        {
            this.udpServer = udpServer;
            this.logger = logger;

            lobbies = new Dictionary<Guid, ILobby>();

            udpServer.OnPacketReceived += OnMessageReceived;
        }

        public Guid CreateLobby()
        {
            Guid id = Guid.NewGuid();

            lock (lobbies)
            {
                lobbies.Add(id, new Lobby(logger, udpServer));
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
                    lobby.OnMessageReceived(packet.Message, packet.ClientSecret, endPoint);
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
                foreach (ILobby lobby in lobbies.Values)
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
