// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class GameTerrainInfo : IMessage
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public Grid<GameCellInfo> Grid { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            Width = reader.ReadByte();
            Height = reader.ReadByte();

            Grid = new Grid<GameCellInfo>(Width, Height);

            for (int i = 0; i < Grid.CellCount; i++)
            {
                var cell = new GameCellInfo();
                cell.ReadFrom(reader);
                Grid[i] = cell;
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)Width);
            writer.Write((byte)Height);

            for (int i = 0; i < Grid.CellCount; i++)
            {
                Grid[i].WriteTo(writer);
            }
        }
    }
}
