// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class LobbyPlayerInfo : IKeyedMessage<Guid>
    {
        public string Name { get; set; }
        public Guid Key { get; set; }
        public byte Index { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            Index = reader.ReadByte();
            Key = reader.ReadGuid();
            Name = reader.ReadString();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Key);
            writer.Write(Name);
        }
    }
}
