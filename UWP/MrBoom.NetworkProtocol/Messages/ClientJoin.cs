// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.IO;
using MrBoom.Common;

namespace MrBoom.NetworkProtocol.Messages
{
    public class ClientJoin : IMessage
    {
        public Guid ClientSecret { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            ClientSecret = reader.ReadGuid();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(ClientSecret);
        }
    }
}
