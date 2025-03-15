// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public interface IClientTerrain : IClientGameEntity
    {
        int Tick { get; }

        int TimeLeft { get; }
        int ApocalypseSpeed { get; }
        int MaxApocalypse { get; }

        int Width { get; }
        int Height { get; }

        int LevelIndex { get; }
        Assets.Level LevelAssets { get; }

        IList<SpriteBase> Sprites { get; }

        Cell GetCell(int x, int y);
        bool IsWalkable(int x, int y);
    }

    public class ClientTerrain : IClientTerrain
    {
        private readonly ITerrainProxy proxy;
        private readonly Assets assets;

        public int Tick { get; private set; }
        public int TimeLeft => proxy.TimeLeft;
        public int ApocalypseSpeed => proxy.ApocalypseSpeed;
        public int MaxApocalypse => proxy.MaxApocalypse;
        public int Width => proxy.Width;
        public int Height => proxy.Height;
        public int LevelIndex => proxy.LevelIndex;
        public Assets.Level LevelAssets => assets.Levels[LevelIndex];

        public IList<SpriteBase> Sprites { get; }

        public ClientTerrain(ITerrainProxy proxy, Assets assets)
        {
            this.proxy = proxy;
            this.assets = assets;

            Tick = 0;
            Sprites = new List<SpriteBase>();
        }

        public Cell GetCell(int x, int y)
        {
            return proxy.GetCell(x, y);
        }

        public bool IsWalkable(int x, int y)
        {
            return proxy.IsWalkable(x, y);
        }

        public void ClientUpdate()
        {
            Tick++;

            foreach (SpriteBase sprite in Sprites)
            {
                sprite.ServerUpdate();
            }

            // sync
        }
    }
}
