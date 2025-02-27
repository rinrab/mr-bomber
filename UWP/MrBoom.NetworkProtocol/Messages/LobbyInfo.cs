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
        public List<LobbyPlayerInfo> Players { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            StartIn = reader.ReadInt32();

            int count = reader.ReadByte();
            Players = new List<LobbyPlayerInfo>(count);

            for (int i = 0; i < count; i++)
            {
                LobbyPlayerInfo player = new LobbyPlayerInfo();
                player.ReadFrom(reader);
                Players.Add(player);
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(StartIn);

            writer.Write((byte)Players.Count);
            foreach (var player in Players)
            {
                player.WriteTo(writer);
            }
        }
    }
}
