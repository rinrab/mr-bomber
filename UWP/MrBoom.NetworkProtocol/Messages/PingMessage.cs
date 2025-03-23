// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class PingMessage : IMessage
    {
        public long PingId { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            PingId = reader.ReadInt64();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(PingId);
        }
    }
}
