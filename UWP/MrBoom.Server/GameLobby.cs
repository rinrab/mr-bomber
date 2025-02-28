// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server
{
    public class GameLobby : ILobbyState
    {
        private readonly List<ClientInfo> clients;
        private readonly List<LobbyPlayer> players;
        private readonly ILobbyStateManager state;
        private readonly ILogger logger;
        private int index = 0;

        private int tick = 0;
        public int StartIn { get; private set; } = -1;

        public Terrain Terrain { get; }

        public GameLobby(ILobbyStateManager state, ILogger logger)
        {
            players = new List<LobbyPlayer>();
            clients = new List<ClientInfo>();

            this.state = state;
            this.logger = logger;

            Terrain = new Terrain(1);
        }

        public LobbyPlayer PlayerJoin(Guid id)
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer("qqq");

            lobbyPlayer.Id = id;
            lobbyPlayer.Index = index;

            players.Add(lobbyPlayer);
            Terrain.AddPlayer(new ServerPlayer(Terrain, index));

            index++;

            return lobbyPlayer;
        }

        public ClientInfo ClientJoin(ClientJoinRequest request, IPEndPoint ipep)
        {
            ClientInfo clientInfo = new ClientInfo()
            {
                ClientSecret = Guid.NewGuid(),
                IpAddress = ipep,
            };

            clients.Add(clientInfo);

            return clientInfo;
        }

        public IEnumerable<ClientInfo> GetClients()
        {
            return clients;
        }

        public IEnumerable<LobbyPlayer> GetPlayers()
        {
            return players;
        }

        public void ServerUpdate()
        {
            tick++;

            if (players.Count >= 2 && StartIn == -1)
            {
                StartIn = 600;
            }

            if (StartIn > 0)
            {
                StartIn--;
            }
        }

        public void OnPacketReceived(Packet packet, IPEndPoint endPoint)
        {
            if (packet.Message is ClientJoin clientJoin)
            {
                ClientJoin(null, endPoint);
            }
            else if (packet.Message is PlayerJoin playerJoin)
            {
                PlayerJoin(playerJoin.Id);
            }
        }

        private IMessage FormatLobbyInfoMessage()
        {
            var players = new List<LobbyPlayerInfo>();

            foreach (var player in GetPlayers())
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
                StartIn = StartIn,
            };
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
            foreach (var client in GetClients())
            {
                await udpServer.SendPacket(new Packet(FormatLobbyInfoMessage()),
                                           client.IpAddress, stoppingToken);

                await udpServer.SendPacket(new Packet(FormatGameInfoMessage()),
                                           client.IpAddress, stoppingToken);
            }
        }
    }
}
