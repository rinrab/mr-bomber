// Copyright (c) Timofei Zhakov. All rights reserved.

using System.IO;
using MrBoom.Common;

namespace MrBoom.NetworkProtocol.Messages
{
    public class ClientJoin : IMessage
    {
        public string ClientSecret { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            ClientSecret = reader.ReadString();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(ClientSecret);
        }
    }
}
