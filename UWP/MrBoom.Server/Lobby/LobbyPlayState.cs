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

            foreach (LobbyPlayer player in lobby.GetPlayers())
            {
                Terrain.AddPlayer(new ServerPlayer(Terrain, player.Index, player.Client.CorishInfo));
            }

            Terrain.InitializeMonsters();
        }

        public void ServerUpdate()
        {
            Terrain.Update();
        }

        public void OnPacketReceived(Packet packet, IPEndPoint endPoint)
        {
            if (packet.Message is ClientUpdateMessage clientUpdate)
            {
                foreach (var spriteUpdate in clientUpdate.SpriteUpdates)
                {
                    var sprite = (IServerPlayer)Terrain.Sprites[spriteUpdate.Index];

                    if (Math.Abs(sprite.X - spriteUpdate.MoveToX) +
                        Math.Abs(sprite.Y - spriteUpdate.MoveToY) < 8)
                    {
                        sprite.MoveTo(spriteUpdate.MoveToX, spriteUpdate.MoveToY);
                    }

                    if (spriteUpdate.DropBomb)
                    {
                        sprite.ToggleDropBomb();
                    }

                    if (spriteUpdate.RemoteControl)
                    {
                        sprite.ToggleRemoteControl();
                    }
                }
            }
        }

        private IMessage FormatGameInfoMessage(ClientInfo client)
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
                var spriteMsg = new GameSpriteInfo
                {
                    X = sprite.X,
                    Y = sprite.Y,
                };

                GameSpriteType type;
                if (sprite is ServerPlayer serverPlayer)
                {
                    if (serverPlayer.ClientInfo.Equals(client.CorishInfo))
                    {
                        type = GameSpriteType.PlayerMe;
                    }
                    else
                    {
                        type = GameSpriteType.Player;
                    }
                }
                else
                {
                    type = GameSpriteType.Monster;
                }

                spriteMsg.Type = type;

                sprites.Add(spriteMsg);
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
                await udpServer.SendPacket(new Packet(FormatGameInfoMessage(client)),
                                           client.IpAddress, stoppingToken);
            }
        }
    }
}
