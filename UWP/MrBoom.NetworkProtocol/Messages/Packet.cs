using System;
using System.IO;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Messages
{
    public class Packet : IMessage
    {
        public IMessage Message { get; set; }

        public Packet()
        {
        }

        public Packet(IMessage message)
        {
            Message = message;
        }

        public void ReadFrom(BinaryReader reader)
        {
            var type = (PacketType)reader.ReadByte();

            if (type == PacketType.PlayerJoin)
            {
                Message = new PlayerJoin();
                Message.ReadFrom(reader);
            }
            else if (type == PacketType.ClientJoin)
            {
                Message = new ClientJoin();
                Message.ReadFrom(reader);
            }
            else
            {
                throw new NetworkException();
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            if (Message is PlayerJoin)
            {
                writer.Write((byte)PacketType.PlayerJoin);
            }
            else if (Message is ClientJoin)
            {
                writer.Write((byte)PacketType.ClientJoin);
            }
            else
            {
                throw new NetworkException();
            }

            Message.WriteTo(writer);
        }
    }
}
