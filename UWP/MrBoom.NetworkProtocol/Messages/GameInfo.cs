// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class GameInfo : IMessage
    {
        public int LevelIndex { get; set; }
        public GameTerrainInfo Terrain { get; set; }
        public List<GameSpriteInfo> Sprites { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            LevelIndex = reader.ReadByte();

            Terrain = new GameTerrainInfo();
            Terrain.ReadFrom(reader);

            int spritesCount = reader.ReadByte();
            Sprites = new List<GameSpriteInfo>();
            for (int i = 0; i < spritesCount; i++)
            {
                var sprite = new GameSpriteInfo();
                sprite.ReadFrom(reader);
                Sprites.Add(sprite);
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)LevelIndex);

            Terrain.WriteTo(writer);

            writer.Write((byte)Sprites.Count);
            foreach (var sprite in Sprites)
            {
                sprite.WriteTo(writer);
            }
        }
    }
}
