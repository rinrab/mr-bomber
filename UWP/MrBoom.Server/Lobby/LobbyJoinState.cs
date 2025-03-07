// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.Common;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public class LobbyJoinState : ILobbyState
    {
        private readonly ILobby lobby;
        private readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;
        private readonly INameGenerator nameGenerator;

        protected int startIn = -1;

        public LobbyJoinState(ILobby lobby, ILogger<LobbyJoinState> logger, IServiceProvider serviceProvider, INameGenerator nameGenerator)
        {
            this.lobby = lobby;
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.nameGenerator = nameGenerator;
        }

        private IMessage FormatLobbyInfoMessage()
        {
            var p = new List<LobbyPlayerInfo>();

            foreach (var player in lobby.GetPlayers())
            {
                p.Add(new LobbyPlayerInfo
                {
                    Key = player.Id,
                    Index = (byte)player.Index,
                    Name = player.Name
                });
            }

            return new LobbyInfo
            {
                Players = new LobbyPlayerCollection
                {
                    Children = p
                },
                StartIn = startIn,
            };
        }

        public void OnMessageReceived(IMessage message, Guid clientSecret, IPEndPoint endPoint)
        {
            if (message is ClientJoin clientJoin)
            {
                lobby.AddClient(new ClientInfo(lobby, endPoint, clientSecret));
            }
            else if (message is PlayerJoin playerJoin)
            {
                ClientInfo? client = lobby.GetClient(clientSecret);

                if (client == null)
                {
                    logger.LogWarning("Rejected client update from {ip}; No client {id} was found.",
                                      endPoint, clientSecret);
                    return;
                    // fuck off mister client. ur fake
                }

                client.OnPacketReceived();

                if (lobby.GetPlayer(playerJoin.Id) == null)
                {
                    lobby.AddPlayer(new LobbyPlayer(nameGenerator.GenerateName())
                    {
                        Id = playerJoin.Id,
                        Index = lobby.GetPlayerCount(),
                        Client = client,
                    });
                }
                else
                {
                    // avoid duplicates
                    // throw this packet away! i dont care!
                }
            }
            else if (message is PingMessage pingMessage)
            {
                ClientInfo? client = lobby.GetClient(clientSecret);

                if (client == null)
                {
                    logger.LogWarning("Rejected client update from {ip}; No client {id} was found.",
                                      endPoint, clientSecret);
                    return;
                    // suce ma bite
                }

                client.OnPacketReceived();
            }
        }

        public async Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken)
        {
            foreach (ClientInfo client in lobby.GetClients())
            {
                await client.SendMessage(FormatLobbyInfoMessage(), stoppingToken);
            }
        }

        public void ServerUpdate()
        {
            lobby.FilterDeadClients();

            if (lobby.GetPlayerCount() >= 2 && startIn == -1)
            {
                startIn = 60 * 3;
            }

            if (startIn == 0)
            {
                lobby.SetState<LobbyPlayState>();
            }
            else if (startIn > 0)
            {
                startIn--;
            }
        }
    }
}
