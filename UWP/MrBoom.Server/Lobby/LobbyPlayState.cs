// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.Common;
using MrBoom.Core.Sprites;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public class LobbyPlayState : ILobbyState
    {
        private readonly ILogger logger;
        private readonly ILobby lobby;

        public Terrain Terrain { get; }

        public LobbyPlayState(ILobby lobby, ILogger<LobbyPlayState> logger, IRandom random)
        {
            this.lobby = lobby;
            this.logger = logger;

            Terrain = new Terrain(1, random);

            foreach (LobbyPlayer player in lobby.GetPlayers())
            {
                var sprite = new ServerPlayer(Terrain, player.Index, player.Index, player.Client.CorishInfo);
                sprite.AddSingleton<PlayerController>();
                Terrain.AddPlayer(sprite);
            }

            Terrain.InitializeMonsters();
        }

        public void ServerUpdate()
        {
            lobby.FilterDeadClients();
            Terrain.Update();
        }

        public void OnMessageReceived(IMessage message, Guid clientSecret, IPEndPoint endPoint)
        {
            if (message is ClientUpdateMessage clientUpdate)
            {
                ClientInfo? client = lobby.GetClient(clientSecret);

                if (client == null)
                {
                    logger.LogWarning("Rejected client update from {ip}; No client {id} was found.",
                                      endPoint, clientSecret);
                    return;
                }

                client.OnPacketReceived();

                foreach (var spriteUpdate in clientUpdate.SpriteUpdates)
                {
                    var sprite = Terrain.GetSprites().ElementAt(spriteUpdate.Index);

                    if (Math.Abs(sprite.GetService<SpritePosition>().X - spriteUpdate.MoveToX) +
                        Math.Abs(sprite.GetService<SpritePosition>().Y - spriteUpdate.MoveToY) < 8)
                    {
                        sprite.GetService<SpritePosition>().MoveTo(spriteUpdate.MoveToX, spriteUpdate.MoveToY);
                    }

                    if (spriteUpdate.DropBomb)
                    {
                        sprite.GetService<PlayerController>().DropBomb();
                    }

                    if (spriteUpdate.RemoteControl)
                    {
                        sprite.GetService<PlayerController>().RemoteDetonate();
                    }
                }
            }
        }

        private GameSpriteType getSpriteType(object sprite, ClientInfo client)
        {
            if (sprite is ServerPlayer serverPlayer)
            {
                if (serverPlayer.ClientInfo.Equals(client.CorishInfo))
                {
                    return GameSpriteType.PlayerMe;
                }
                else
                {
                    return GameSpriteType.Player;
                }
            }
            else
            {
                return GameSpriteType.Monster;
            }
        }

        private IMessage FormatGameInfoMessage(ClientInfo client)
        {
            var grid = new Grid<GameCellInfo>(Terrain.Width, Terrain.Height);
            for (int i = 0; i < grid.CellCount; i++)
            {
                Cell cell = Terrain.GetCell(grid.GetCellX(i), grid.GetCellY(i));

                int subType = 0;
                if (cell.Type == TerrainType.PowerUp)
                {
                    subType = (int)cell.PowerUpType;
                }
                else if (cell.Type == TerrainType.Fire)
                {
                    subType = (int)cell.FlameDirection;
                }

                grid[i] = new GameCellInfo
                {
                    Type = cell.Type,
                    SubType = subType,
                    Index = cell.Index,
                    AnimateDelay = cell.animateDelay,
                    OffsetX = cell.OffsetX,
                    OffsetY = cell.OffsetY,
                };
            }

            var sprites = new List<GameSpriteInfo>();
            foreach (SpriteBase sprite in Terrain.GetSprites())
            {
                sprites.Add(new GameSpriteInfo
                {
                    X = sprite.GetService<SpritePosition>().X,
                    Y = sprite.GetService<SpritePosition>().Y,
                    Type = getSpriteType(sprite, client),
                    SubType = sprite.GetService<SpriteStartInfo>().SubType,
                    AnimateIndex = sprite.GetService<SpriteAnimationController>().AnimateIndex,
                    FrameIndex = sprite.GetService<SpriteAnimationController>().FrameIndex,
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
                await client.SendMessage(FormatGameInfoMessage(client), stoppingToken);
            }
        }
    }
}
