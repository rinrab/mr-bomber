// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class ClientPlayerUpdateMessage : IMessage
    {
        public byte Index { get; set; }

        public int MoveToX { get; set; }
        public int MoveToY { get; set; }

        public bool DropBomb { get; set; }
        public bool RemoteControl { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            Index = reader.ReadByte();

            MoveToX = reader.ReadInt32();
            MoveToY = reader.ReadInt32();

            DropBomb = reader.ReadBoolean();
            RemoteControl = reader.ReadBoolean();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Index);

            writer.Write(MoveToX);
            writer.Write(MoveToY);

            writer.Write(DropBomb);
            writer.Write(RemoteControl);
        }
    }
}
