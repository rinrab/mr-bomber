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
                terrainProxy.SetIncomingMessage(gameInfo);

                if (clientTerrain == null)
                {
                    InitializeTerrain();
                }
            }
        }

        private void InitializeTerrain()
        {
            clientTerrain = new ClientTerrain(terrainProxy, assets);

            for (int i = 0; i < terrainProxy.Sprites.Count; i++)
            {
                ISpriteProxy proxy = terrainProxy.Sprites[i];

                // TODO: verify sprite type

                if (proxy is IPlayerProxy playerProxy)
                {
                    IPlayerState player = players[i];

                    if (player is OnlineLocalPlayerState onlinePlayer)
                    {
                        clientTerrain.Sprites.Add(new ClientSpriteLocalHuman(terrainProxy, playerProxy, assets, onlinePlayer.Controller));
                    }
                    else
                    {
                        clientTerrain.Sprites.Add(new ClientSprite(proxy, assets));
                    }
                }
                else
                {
                    clientTerrain.Sprites.Add(new ClientSprite(proxy, assets));
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
