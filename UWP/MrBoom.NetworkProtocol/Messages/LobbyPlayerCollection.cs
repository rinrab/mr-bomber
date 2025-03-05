// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class LobbyPlayerCollection : IKeyedMessageCollection<Guid, LobbyPlayerInfo>, IMessage
    {
        public IList<LobbyPlayerInfo> Children { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            int count = reader.ReadByte();

            Children = new List<LobbyPlayerInfo>(count);

            for (int i = 0; i < count; i++)
            {
                LobbyPlayerInfo player = new LobbyPlayerInfo();
                player.ReadFrom(reader);
                Children.Add(player);
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)Children.Count);

            foreach (LobbyPlayerInfo player in Children)
            {
                player.WriteTo(writer);
            }
        }
    }
}
