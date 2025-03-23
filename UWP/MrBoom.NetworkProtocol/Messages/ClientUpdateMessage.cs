// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.IO;

namespace MrBoom.NetworkProtocol.Messages
{
    public class ClientUpdateMessage : IMessage
    {
        public IList<ClientPlayerUpdateMessage> SpriteUpdates { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            int count = reader.ReadByte();
            SpriteUpdates = new List<ClientPlayerUpdateMessage>(count);
            for (int i = 0; i < count; i++)
            {
                var spriteUpdate = new ClientPlayerUpdateMessage();
                spriteUpdate.ReadFrom(reader);
                SpriteUpdates.Add(spriteUpdate);
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)SpriteUpdates.Count);
            foreach (var spriteUpdate in SpriteUpdates)
            {
                spriteUpdate.WriteTo(writer);
            }
        }
    }
}
