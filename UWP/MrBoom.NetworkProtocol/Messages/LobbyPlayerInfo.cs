// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.IO;
using MrBoom.Common;

namespace MrBoom.NetworkProtocol.Messages
{
    public class LobbyPlayerInfo : IMessage
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public byte Index { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            Index = reader.ReadByte();
            Id = new Guid(reader.ReadBytes(16));
            Name = reader.ReadString();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Id.ToByteArray());
            writer.Write(Name);
        }
    }
}
