// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.NetworkProtocol.Proxy;

namespace MrBoom.Screens
{
    public class OnlineGameScreen : ClientGameScreen
    {
        private readonly MultiplayerClient multiplayerClient;
        private readonly TerrainProxy terrainProxy;

        public OnlineGameScreen(Assets assets, MultiplayerClient multiplayerClient) : base(assets)
        {
            terrainProxy = new TerrainProxy();
            clientTerrain = new ClientTerrain(terrainProxy, assets);

            this.multiplayerClient = multiplayerClient;
            multiplayerClient.OnPacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(Packet packet)
        {
            if (packet.Message is GameInfo gameInfo)
            {
                terrainProxy.Message = gameInfo;
            }
        }

        public override void Update()
        {
            multiplayerClient.CheckPackets();

            if (terrainProxy.Message != null)
            {
                terrainProxy.ClientUpdate();
                base.Update();
            }
        }

        public override void Draw(SpriteBatch ctx)
        {
            if (terrainProxy.Message != null)
            {
                base.Draw(ctx);
            }
        }
    }
}
