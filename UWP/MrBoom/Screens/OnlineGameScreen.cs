// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Screens
{
    public class TerrainProxy : ITerrainProxy
    {
        public GameInfo Message;

        public int TimeLeft { get; set; }
        public int ApocalypseSpeed { get; set; }
        public int MaxApocalypse { get; set; }
        public int Width => Message.Terrain.Width;
        public int Height => Message.Terrain.Width;
        public int LevelIndex => Message.LevelIndex;

        public IList<ISprite> Sprites { get; }

        public TerrainProxy()
        {
            Sprites = new List<ISprite>();
        }

        public void ClientUpdate()
        {
        }

        public Cell GetCell(int x, int y)
        {
            var cell = Message.Terrain.Grid[x, y];

            if (cell == null)
            {
                return new Cell(TerrainType.PermanentWall);
            }
            else
            {
                return new Cell(cell.Type)
                {
                };
            }
        }

        public bool IsWalkable(int x, int y)
        {
            Cell cell = GetCell(x, y);

            switch (cell.Type)
            {
                case TerrainType.Free:
                case TerrainType.PowerUpFire:
                    return true;

                case TerrainType.PermanentWall:
                case TerrainType.Rubber:
                case TerrainType.Apocalypse:
                    return false;

                case TerrainType.TemporaryWall:
                case TerrainType.Bomb:
                    return false; // cheats.noClip

                default:
                    return true;
            }
        }
    }

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
