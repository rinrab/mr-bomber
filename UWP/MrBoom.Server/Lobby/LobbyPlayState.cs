// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.Common;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;
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
            Terrain.AddSingleton<TerrainUpdateBroadcaster>();

            var sprites = Terrain.GetService<TerrainSpriteHost>();

            foreach (LobbyPlayer player in lobby.GetPlayers())
            {
                var sprite = new ServerPlayer(player.Index, player.Index, player.Client.CorishInfo);

                sprite.AddSingleton(player.Client.CorishInfo);
                sprite.AddSingleton<SpriteTypeProviderPlayer>();
                sprite.AddSingleton<SpriteUpdateReceiver>();
                sprite.AddSingleton<SpriteUpdateBroadcaster>();

                sprites.AddPlayer(sprite);
            }

            sprites.InitializeMonsters();

            foreach (AbstractMonster monster in sprites.GetMonsters())
            {
                monster.AddSingleton<SpriteTypeProviderMonster>();
                monster.AddSingleton<SpriteUpdateBroadcaster>();
            }
        }

        public void ServerUpdate()
        {
            lobby.FilterDeadClients();
            Terrain.ServerUpdate();
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

                var sprites = Terrain.GetService<TerrainSpriteHost>();

                foreach (ClientPlayerUpdateMessage spriteUpdate in clientUpdate.SpriteUpdates)
                {
                    GameEntityBase sprite = sprites.GetSprites().ElementAt(spriteUpdate.Index);
                    // TODO: authenticate
                    sprite.GetService<SpriteUpdateReceiver>().OnUpdateReceived(spriteUpdate);
                }
            }
        }

        public async Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken)
        {
            foreach (ClientInfo client in lobby.GetClients())
            {
                TerrainUpdateBroadcaster broadcaster = Terrain.GetService<TerrainUpdateBroadcaster>();
                await client.SendMessage(broadcaster.GetUpdateMessage(client), stoppingToken);
            }

            Terrain.GetService<BasicSoundController>().ResetSounds();
        }
    }
}
