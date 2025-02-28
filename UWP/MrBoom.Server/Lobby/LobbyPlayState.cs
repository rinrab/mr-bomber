// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Net;
using MrBoom.NetworkProtocol;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public class LobbyPlayState : ILobbyState
    {
        private readonly ILobbyStateManager state;
        private readonly ILogger logger;
        private readonly ILobby lobby;

        public Terrain Terrain { get; }

        public LobbyPlayState(ILobbyStateManager state, ILogger logger, ILobby lobby)
        {
            this.state = state;
            this.logger = logger;
            this.lobby = lobby;

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
            foreach (ClientInfo client in lobby.GetClients())
            {
                await udpServer.SendPacket(new Packet(FormatGameInfoMessage()),
                                           client.IpAddress, stoppingToken);
            }
        }
    }
}
