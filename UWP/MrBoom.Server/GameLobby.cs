// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Net;
using MrBoom.NetworkProtocol;
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
                state.SetState(new GameLobby(state, logger, clients, players));
            }
            else if (startIn > 0)
            {
                startIn--;
            }
        }
    }

    public class GameLobby : ILobbyState
    {
        private readonly ILobbyStateManager state;
        private readonly ILogger logger;
        private readonly List<ClientInfo> clients;
        private readonly List<LobbyPlayer> players;

        public Terrain Terrain { get; }

        public GameLobby(ILobbyStateManager state, ILogger logger, List<ClientInfo> clients, List<LobbyPlayer> players)
        {
            this.state = state;
            this.logger = logger;
            this.clients = clients;
            this.players = players;

            Terrain = new Terrain(1);
        }


        public void ServerUpdate()
        {
        }

        public void OnPacketReceived(Packet packet, IPEndPoint endPoint)
        {
        }

        private IMessage FormatGameInfoMessage()
        {
            var grid = new Grid<GameCellInfo>(Terrain.Width, Terrain.Height);
            for (int i = 0; i < grid.CellCount; i++)
            {
                Cell cell = Terrain.GetCell(grid.GetCellX(i), grid.GetCellY(i));

                grid[i] = new GameCellInfo
                {
                    Type = cell.Type,
                };
            }

            var sprites = new List<GameSpriteInfo>();
            foreach (Sprite sprite in Terrain.GetSprites())
            {
                sprites.Add(new GameSpriteInfo
                {
                    X = sprite.X,
                    Y = sprite.Y,
                });
            }

            return new GameInfo
            {
                LevelIndex = Terrain.LevelIndex,
                Terrain = new GameTerrainInfo
                {
                    Width = Terrain.Width,
                    Height = Terrain.Height,
                    Grid = grid,
                },
                Sprites = sprites,
            };
        }

        public async Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken)
        {
            foreach (ClientInfo client in clients)
            {
                await udpServer.SendPacket(new Packet(FormatGameInfoMessage()),
                                           client.IpAddress, stoppingToken);
            }
        }
    }
}
