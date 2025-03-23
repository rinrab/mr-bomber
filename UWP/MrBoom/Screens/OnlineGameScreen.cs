// Copyright (c) Timofei Zhakov. All rights reserved.

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;
using MrBoom.State;

namespace MrBoom.Screens
{
    public class OnlineGameScreen : IScreen
    {
        private readonly MultiplayerClient multiplayerClient;
        private readonly TerrainProxy terrainProxy;

        private readonly IPlayerProvider players;
        private readonly ISpriteProvider sprites;

        private ClientTerrain clientTerrain;
        private readonly Assets assets;

        public OnlineGameScreen(Assets assets, MultiplayerClient multiplayerClient, IPlayerProvider players)
        {
            terrainProxy = new TerrainProxy();

            this.assets = assets;
            this.multiplayerClient = multiplayerClient;
            this.players = players;

            sprites = new SpriteProvider(players);

            multiplayerClient.OnPacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(Packet packet)
        {
            if (packet.Message is GameInfo gameInfo)
            {
                bool init = false;

                if (clientTerrain == null)
                {
                    init = true;

                    clientTerrain = new ClientTerrain(terrainProxy, assets);

                    foreach (IPlayerState state in sprites.EnumerateSprites())
                    {
                        terrainProxy.Sprites.Add(state.InitializeProxy(ExtensibilityProvider.Default));
                    }
                }

                terrainProxy.SetIncomingMessage(gameInfo);

                if (init)
                {
                    ClientTerrainSpriteHost clientSprites = clientTerrain.GetService<ClientTerrainSpriteHost>();

                    foreach (IPlayerState state in sprites.EnumerateSprites())
                    {
                        clientSprites.Sprites.Add(state.InitializeClientSprite(terrainProxy, assets));
                    }
                }
            }
        }

        public void Update()
        {
            multiplayerClient.CheckPackets();

            if (clientTerrain != null)
            {
                terrainProxy.ClientUpdate();

                _ = multiplayerClient.SendPacket(new Packet(terrainProxy.GetOutcomingMessage())
                {
                    Lobby = multiplayerClient.LobbyId,
                    ClientSecret = multiplayerClient.ClientSecret,
                });

                clientTerrain.ServerUpdate();
            }

            if (multiplayerClient.IsDead())
            {
                ScreenManager.SetScreen(new OnlineConnectionDiedScreen(assets));
            }
        }

        public void Draw(SpriteBatch ctx)
        {
            if (clientTerrain != null)
            {
                clientTerrain.Draw(ctx);
            }
        }

        public void DrawHighDPI(SpriteBatch ctx, Rectangle rect, float scale, int graphicScale)
        {
        }
    }
}
