// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public class LobbyJoinState : ILobbyState
    {
        private readonly ILobbyStateManager state;
        private readonly ILogger logger;

        private readonly ILobby lobby;

        protected int startIn = -1;

        public LobbyJoinState(ILobbyStateManager state, ILogger logger)
        {
            this.state = state;
            this.logger = logger;

            lobby = new Lobby(logger);
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

        public void OnPacketReceived(Packet packet, IPEndPoint endPoint)
        {
            if (packet.Message is ClientJoin clientJoin)
            {
                lobby.AddClient(new ClientInfo
                {
                    ClientSecret = clientJoin.ClientSecret,
                    IpAddress = endPoint
                });
            }
            else if (packet.Message is PlayerJoin playerJoin)
            {
                ClientInfo? client = lobby.GetClient(playerJoin.ClientSecret);

                if (client == null)
                {
                    return; // fuck off mister client. ur fake
                }

                lobby.AddPlayer(new LobbyPlayer("qqq")
                {
                    Id = playerJoin.Id,
                    Index = lobby.GetPlayerCount(),
                    Client = client,
                });
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
                state.SetState(new LobbyPlayState(state, logger, lobby));
            }
            else if (startIn > 0)
            {
                startIn--;
            }
        }
    }
}
