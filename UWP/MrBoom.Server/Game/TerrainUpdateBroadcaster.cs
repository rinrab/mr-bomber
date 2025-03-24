// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.Server.Lobby;

namespace MrBoom.Server.Game
{
    public class TerrainUpdateBroadcaster
    {
        private readonly TerrainMap map;
        private readonly TerrainSpriteHost terrainSprites;
        private readonly TerrainStartInfo startInfo;
        private readonly BasicSoundController soundController;

        public TerrainUpdateBroadcaster(TerrainMap map,
                                        TerrainSpriteHost terrainSprites,
                                        TerrainStartInfo startInfo,
                                        BasicSoundController soundController)
        {
            this.map = map;
            this.terrainSprites = terrainSprites;
            this.startInfo = startInfo;
            this.soundController = soundController;
        }

        public GameInfo GetUpdateMessage(ClientInfo client)
        {
            var grid = new Grid<GameCellInfo>(map.Width, map.Height);
            for (int i = 0; i < grid.CellCount; i++)
            {
                Cell cell = map.GetCell(grid.GetCellX(i), grid.GetCellY(i));

                int subType = 0;
                if (cell.Type == TerrainType.PowerUp)
                {
                    subType = (int)cell.PowerUpType;
                }
                else if (cell.Type == TerrainType.Fire)
                {
                    subType = (int)cell.FlameDirection;
                }

                grid[i] = new GameCellInfo
                {
                    Type = cell.Type,
                    SubType = subType,
                    Index = cell.Index,
                    AnimateDelay = cell.animateDelay,
                    OffsetX = cell.OffsetX,
                    OffsetY = cell.OffsetY,
                };
            }

            var sprites = new List<GameSpriteInfo>();
            foreach (GameEntityBase sprite in terrainSprites.GetSprites())
            {
                sprites.Add(sprite.GetService<SpriteUpdateBroadcaster>().GetUpdateMessage(client));
            }

            return new GameInfo
            {
                LevelIndex = startInfo.LevelIndex,
                SoundsToPlay = soundController.SoundsToPlay,
                Terrain = new GameTerrainInfo
                {
                    Width = map.Width,
                    Height = map.Height,
                    Grid = grid,
                },
                Sprites = sprites,
            };
        }
    }
}
