// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public class LobbyJoinState : ILobbyState
    {
        private readonly ILobby lobby;
        private readonly ILogger logger;

        protected int startIn = -1;

        public LobbyJoinState(ILobby lobby, ILogger logger)
        {
            this.lobby = lobby;
            this.logger = logger;
        }

        private IMessage FormatLobbyInfoMessage()
        {
            var p = new List<LobbyPlayerInfo>();

            foreach (var player in lobby.GetPlayers())
            {
                p.Add(new LobbyPlayerInfo
                {
                    Id = player.Id,
                    Index = (byte)player.Index,
                    Name = player.Name
                });
            }

            return new LobbyInfo
            {
                Players = p,
                StartIn = startIn,
            };
        }

        public void OnMessageReceived(IMessage message, Guid clientSecret, IPEndPoint endPoint)
        {
            if (message is ClientJoin clientJoin)
            {
                lobby.AddClient(new ClientInfo(endPoint, clientSecret));
            }
            else if (message is PlayerJoin playerJoin)
            {
                ClientInfo? client = lobby.GetClient(clientSecret);

                if (client == null)
                {
                    return; // fuck off mister client. ur fake
                }

                client.OnPacketReceived();

                lobby.AddPlayer(new LobbyPlayer("qqq")
                {
                    Id = playerJoin.Id,
                    Index = lobby.GetPlayerCount(),
                    Client = client,
                });
            }
            else if (message is PingMessage pingMessage)
            {
                ClientInfo? client = lobby.GetClient(clientSecret);

                if (client == null)
                {
                    return; // suce ma bite
                }

                client.OnPacketReceived();
            }
        }

        public async Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken)
        {
            foreach (ClientInfo client in lobby.GetClients())
            {
                await udpServer.SendPacket(new Packet(FormatLobbyInfoMessage()),
                                           client.IpAddress, stoppingToken);
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
                lobby.State.SetState(new LobbyPlayState(lobby, logger));
            }
            else if (startIn > 0)
            {
                startIn--;
            }
        }
    }
}
