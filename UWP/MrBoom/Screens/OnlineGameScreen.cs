// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;
using MrBoom.State;

namespace MrBoom.Screens
{
    public class OnlineGameScreen : ClientGameScreen
    {
        private readonly MultiplayerClient multiplayerClient;
        private readonly TerrainProxy terrainProxy;

        private readonly IPlayerProvider players;
        private readonly ISpriteProvider sprites;

        public OnlineGameScreen(Assets assets, MultiplayerClient multiplayerClient, IPlayerProvider players) : base(assets)
        {
            terrainProxy = new TerrainProxy();

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
                    foreach (IPlayerState state in sprites.EnumerateSprites())
                    {
                        clientTerrain.Sprites.Add(state.InitializeClientSprite(terrainProxy, assets));
                    }
                }
            }
        }

        public override void Update()
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

                base.Update();
            }

            if (multiplayerClient.IsDead())
            {
                ScreenManager.SetScreen(new OnlineConnectionDiedScreen(assets));
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            if (clientTerrain != null)
            {
                base.Draw(ctx);
            }
        }
    }
}
