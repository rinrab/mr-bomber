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
            else if (packet.Message is PlayerJoin playerJoin)
            {
                lobby.PlayerJoin(playerJoin.Id);
            }
        }

        private IMessage FormatLobbyInfoMessage()
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

            return new LobbyInfo
            {
                Players = players,
                StartIn = lobby.StartIn,
            };
        }

        private IMessage FormatGameInfoMessage()
        {
            var grid = new Grid<GameCellInfo>(lobby.Terrain.Width, lobby.Terrain.Height);
            for (int i = 0; i < grid.CellCount; i++)
            {
                Cell cell = lobby.Terrain.GetCell(grid.GetCellX(i), grid.GetCellY(i));

                grid[i] = new GameCellInfo
                {
                    Type = cell.Type,
                };
            }

            var sprites = new List<GameSpriteInfo>();
            foreach (Sprite sprite in lobby.Terrain.GetSprites())
            {
                sprites.Add(new GameSpriteInfo
                {
                    X = sprite.X,
                    Y = sprite.Y,
                });
            }

            return new GameInfo
            {
                LevelIndex = lobby.Terrain.LevelIndex,
                Terrain = new GameTerrainInfo
                {
                    Width = lobby.Terrain.Width,
                    Height = lobby.Terrain.Height,
                    Grid = grid,
                },
                Sprites = sprites,
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                lobby.ServerUpdate();

                foreach (var client in lobby.GetClients())
                {
                    _ = udpServer.SendPacket(new Packet(FormatLobbyInfoMessage()),
                                             client.IpAddress, stoppingToken);

                    _ = udpServer.SendPacket(new Packet(FormatGameInfoMessage()),
                                             client.IpAddress, stoppingToken);
                }

                await Task.Delay(1000 / 60, stoppingToken);
            }
        }
    }
}
