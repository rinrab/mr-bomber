// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;
using Haukcode.HighResolutionTimer;
using MrBoom.Server.MatchMaking;
using MrBoom.Server.Admin;
using Microsoft.Extensions.Options;

namespace MrBoom.Server.Lobby
{
    public class LobbyServer : TimerService, ILobbyProvider, IAdminLobbyProvider
    {
        private readonly IUdpServer udpServer;
        private readonly ILogger logger;
        private readonly IOptions<Settings> options;
        private readonly IServiceProvider serviceProvider;

        private readonly Dictionary<Guid, Lobby> lobbies;

        public LobbyServer(IUdpServer udpServer,
                           ILogger<LobbyServer> logger,
                           IOptions<Settings> options,
                           IServiceProvider serviceProvider) : base(1000 / 20, logger)
        {
            this.udpServer = udpServer;
            this.logger = logger;
            this.options = options;
            this.serviceProvider = serviceProvider;

            lobbies = new Dictionary<Guid, Lobby>();

            udpServer.OnPacketReceived += OnMessageReceived;
        }

        public Guid CreateLobby()
        {
            var lobby = ActivatorUtilities.CreateInstance<Lobby>(serviceProvider);

            lock (lobbies)
            {
                lobbies.Add(lobby.Key, lobby);
            }

            logger.LogInformation("Created lobby: {masterUrl}/Lobby/{lobby}", options.Value.MasterUrl, lobby.Key);

            return lobby.Key;
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

        public IEnumerable<IMatchMakingLobbyInfo> EnumerateLobbies()
        {
            foreach (IMatchMakingLobbyInfo lobby in lobbies.Values)
            {
                yield return lobby;
            }
        }

        IEnumerable<ILobby> IAdminLobbyProvider.EnumerateLobbies()
        {
            foreach (ILobby lobby in lobbies.Values)
            {
                yield return lobby;
            }
        }

        public ILobby GetLobby(Guid key)
        {
            if (lobbies.TryGetValue(key, out var lobby))
            {
                return lobby;
            }
            else
            {
                throw new Exception("Lobby not found");
            }
        }
    }
}
