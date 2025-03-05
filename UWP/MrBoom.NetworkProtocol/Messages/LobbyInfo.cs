// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MrBoom.Common;

namespace MrBoom.NetworkProtocol.Messages
{
    public class LobbyInfo : IMessage
    {
        public int StartIn { get; set; }
        public LobbyPlayerCollection Players { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            StartIn = reader.ReadInt32();

            Players = new LobbyPlayerCollection();
            Players.ReadFrom(reader);
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(StartIn);

            Players.WriteTo(writer);
        }
    }
}
