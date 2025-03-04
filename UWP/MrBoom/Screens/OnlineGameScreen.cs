// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
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
        private readonly List<IPlayerState> players;
        private readonly TerrainProxy terrainProxy;

        public OnlineGameScreen(Assets assets, MultiplayerClient multiplayerClient, List<IPlayerState> players) : base(assets)
        {
            terrainProxy = new TerrainProxy();

            this.multiplayerClient = multiplayerClient;
            this.players = players;

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

                    foreach (IPlayerState state in players)
                    {
                        terrainProxy.Sprites.Add(state.InitializeProxy());
                    }

                    for (int i = terrainProxy.Sprites.Count - 1; i < gameInfo.Sprites.Count; i++)
                    {
                        terrainProxy.Sprites.Add(new SpriteProxy());
                    }
                }

                terrainProxy.SetIncomingMessage(gameInfo);

                if (init)
                {
                    foreach (IPlayerState state in players)
                    {
                        clientTerrain.Sprites.Add(state.InitializeClientSprite(terrainProxy, assets));
                    }

                    for (int i = clientTerrain.Sprites.Count - 1; i < gameInfo.Sprites.Count; i++)
                    {
                        clientTerrain.Sprites.Add(new ClientSprite(terrainProxy.Sprites[i], assets));
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
