// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

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

        IList<IClientSprite> Sprites { get; }

        Cell GetCell(int x, int y);
        bool IsWalkable(int x, int y);
    }

    public class ClientTerrain : IClientTerrain, ITerrainAccessor
    {
        private readonly ITerrain proxy;

        public int Tick { get; private set; }
        public int TimeLeft => proxy.TimeLeft;
        public int ApocalypseSpeed => proxy.ApocalypseSpeed;
        public int MaxApocalypse => proxy.MaxApocalypse;
        public int Width => proxy.Width;
        public int Height => proxy.Height;
        public int LevelIndex => proxy.LevelIndex;
        public Assets.Level LevelAssets { get; }

        public IList<IClientSprite> Sprites { get; }

        public ClientTerrain(ITerrain proxy, Assets assets)
        {
            this.proxy = proxy;
            Tick = 0;
            LevelAssets = assets.Levels[LevelIndex];

            Sprites = new List<IClientSprite>();
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

            foreach (IClientSprite sprite in Sprites)
            {
                sprite.ClientUpdate();
            }

            // sync
        }
    }
}
