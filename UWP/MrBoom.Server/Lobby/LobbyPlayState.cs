// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.Common;
using MrBoom.Core.Sprites;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.Server.Game;

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

                sprite.AddSingleton(player.Client.CorishInfo);
                sprite.AddSingleton<PlayerController>();
                sprite.AddSingleton<SpriteTypeProviderPlayer>();

                Terrain.AddPlayer(sprite);
            }

            Terrain.InitializeMonsters();

            foreach (AbstractMonster monster in Terrain.GetMonsters())
            {
                monster.AddSingleton<SpriteTypeProviderMonster>();
            }

            foreach (SpriteBase sprite in Terrain.GetSprites())
            {
                sprite.AddSingleton<SpriteUpdateBroadcaster>();
            }
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

                foreach (ClientPlayerUpdateMessage spriteUpdate in clientUpdate.SpriteUpdates)
                {
                    SpriteBase sprite = Terrain.GetSprites().ElementAt(spriteUpdate.Index);
                    // TODO: authenticate
                    sprite.GetService<SpriteUpdateReceiver>().OnUpdateReceived(spriteUpdate);
                }
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
                sprites.Add(sprite.GetService<SpriteUpdateBroadcaster>().GetUpdateMessage(client));
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
