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

            int i = 0;
            foreach (var spriteProxy in terrainProxy.Sprites)
            {
                if (spriteProxy is IPlayerProxy playerProxy)
                {
                    IPlayerState player = players[i];

                    clientTerrain.Sprites.Add(new ClientSpriteLocalHuman(terrainProxy, 0, 0,
                                                                         playerProxy,
                                                                         assets.Players[i],
                                                                         ((OnlinePlayerState)player).Controller));
                }
                else
                {
                    clientTerrain.Sprites.Add(new ClientSprite(spriteProxy, assets.Players[i]));
                }

                i++;
            }
        }

        public override void Update()
        {
            multiplayerClient.CheckPackets();

            if (clientTerrain != null)
            {
                terrainProxy.ClientUpdate();
                base.Update();
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
