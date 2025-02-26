using System;
using System.IO;
using MrBoom.Common.Messages;

namespace MrBoom.Common
{
    class Packet : IMessage
    {
        PacketType Type { get; set; }
        IMessage Message { get; set; }

        public Packet(PacketType type, IMessage message)
        {
            Type = type;
            Message = message;
        }

        public void ReadFrom(BinaryReader reader)
        {
            Type = (PacketType)reader.ReadByte();

            if (Type == PacketType.PlayerJoin)
            {
                Message = new PlayerJoin();
                Message.ReadFrom(reader);
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            Message.WriteTo(writer);
        }
    }
}
