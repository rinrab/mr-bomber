// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class GameSpriteInfo : IMessage
    {
        public int X { get; set; }
        public int Y { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            X = reader.ReadUInt16();
            Y = reader.ReadUInt16();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((ushort)X);
            writer.Write((ushort)Y);
        }
    }
}
