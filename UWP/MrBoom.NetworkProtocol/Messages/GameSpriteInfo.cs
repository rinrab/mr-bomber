// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public enum GameSpriteType
    {
        Monster,
        Player,
    }

    public class GameSpriteInfo : IMessage
    {
        public GameSpriteType Type { get; set; }
        public byte Index { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            Type = (GameSpriteType)reader.ReadByte();
            Index = reader.ReadByte();

            X = reader.ReadUInt16();
            Y = reader.ReadUInt16();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write(Index);

            writer.Write((ushort)X);
            writer.Write((ushort)Y);
        }
    }
}
