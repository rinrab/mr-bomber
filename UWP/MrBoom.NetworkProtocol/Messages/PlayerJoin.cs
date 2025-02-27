using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MrBoom.NetworkProtocol.Messages
{
    class PlayerJoin : IMessage
    {
        public Guid Id { get; set; }

        public void ReadFrom(BinaryReader reader)
        {
            Id = reader.ReadGuid();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Id);
        }
    }
}
