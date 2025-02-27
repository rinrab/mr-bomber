using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MrBoom.NetworkProtocol.Messages
{
    class PlayerJoin : IMessage
    {
        public string Name { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            Name = reader.ReadString();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
}
