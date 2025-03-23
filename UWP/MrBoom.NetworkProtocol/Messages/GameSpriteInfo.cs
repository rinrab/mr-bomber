// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public enum GameSpriteType
    {
        Monster,
        Player,
        PlayerMe,
    }

    public class GameSpriteInfo : IMessage
    {
        public GameSpriteType Type { get; set; }
        public int SubType { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        public int AnimateIndex { get; set; }
        public int FrameIndex { get; set; }

        public bool IsDie { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            Type = (GameSpriteType)reader.ReadByte();
            SubType = reader.ReadByte();

            X = reader.ReadUInt16();
            Y = reader.ReadUInt16();

            AnimateIndex = reader.ReadByte();
            FrameIndex = reader.ReadUInt16();

            IsDie = reader.ReadBoolean();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write((byte)SubType);

            writer.Write((ushort)X);
            writer.Write((ushort)Y);

            writer.Write((byte)(AnimateIndex));
            writer.Write((ushort)(FrameIndex));

            writer.Write(IsDie);
        }
    }
}
