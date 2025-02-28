// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server
{
    public class LobbyJoinState : ILobbyState
    {
        private readonly ILobbyStateManager state;
        private readonly ILogger logger;

        private readonly List<ClientInfo> clients;
        private readonly List<LobbyPlayer> players;

        protected int startIn = -1;

        public LobbyJoinState(ILobbyStateManager state, ILogger logger)
        {
            this.state = state;
            this.logger = logger;

            clients = new List<ClientInfo>();
            players = new List<LobbyPlayer>();
        }

        private IMessage FormatLobbyInfoMessage()
        {
            var p = new List<LobbyPlayerInfo>();

            foreach (var player in players)
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
                clients.Add(new ClientInfo
                {
                    ClientSecret = clientJoin.ClientSecret,
                    IpAddress = endPoint
                });
            }
            else if (packet.Message is PlayerJoin playerJoin)
            {
                players.Add(new LobbyPlayer("qqq")
                {
                    Id = playerJoin.Id,
                    Index = players.Count,
                });
           }
        }

        public async Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken)
        {
            foreach (ClientInfo client in clients)
            {
                await udpServer.SendPacket(new Packet(FormatLobbyInfoMessage()),
                                           client.IpAddress, stoppingToken);
            }
        }

        public void ServerUpdate()
        {
            if (players.Count >= 2 && startIn == -1)
            {
                startIn = 600;
            }

            if (startIn == 0)
            {
                state.SetState(new LobbyPlayState(state, logger, clients, players));
            }
            else if (startIn > 0)
            {
                startIn--;
            }
        }
    }
}
