// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class GameCellInfo : IMessage
    {
        public TerrainType Type;
        public int SubType;

        public int Index;
        public int AnimateDelay;

        public int OffsetX;
        public int OffsetY;

        public void ReadFrom(BinaryReader reader)
        {
            var byte0 = reader.ReadByte();
            var byte1 = reader.ReadByte();
            var byte2 = reader.ReadByte();

            Type = (TerrainType)(byte0 / 16);
            SubType = byte0 % 16;

            OffsetX = (byte)(byte1 / 16) - 8;
            OffsetY = (byte)(byte1 % 16) - 8;

            Index = (byte)(byte2 / 16) - 1;
            AnimateDelay = (byte)(byte2 % 16);
        }

        public void WriteTo(BinaryWriter writer)
        {
            byte byte0 = (byte)((byte)Type * 16 + SubType);

            byte ox = (byte)Math.Clamp(OffsetX + 8, 0, 15);
            byte oy = (byte)Math.Clamp(OffsetY + 8, 0, 15);

            byte byte1 = (byte)(ox * 16 + oy);

            byte index = (byte)(Index + 1);
            byte byte2 = (byte)(index * 16 + AnimateDelay);

            writer.Write(byte0);
            writer.Write(byte1);
            writer.Write(byte2);
        }
    }
}
