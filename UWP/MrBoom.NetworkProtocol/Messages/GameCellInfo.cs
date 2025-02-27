// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class GameCellInfo : IMessage
    {
        public TerrainType Type;

        public void ReadFrom(BinaryReader reader)
        {
            Type = (TerrainType)reader.ReadByte();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)Type);
        }
    }
}
