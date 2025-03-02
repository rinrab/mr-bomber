using System;
using System.IO;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.NetworkProtocol.Messages
{
    public class Packet : IMessage
    {
        public IMessage Message { get; set; }
        public Guid Lobby { get; set; }

        public Packet()
        {
        }

        public Packet(IMessage message)
        {
            Message = message;
        }

        public void ReadFrom(BinaryReader reader)
        {
            Lobby = reader.ReadGuid();

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
            else if (type == PacketType.LobbyInfo)
            {
                Message = new LobbyInfo();
                Message.ReadFrom(reader);
            }
            else if (type == PacketType.GameInfo)
            {
                Message = new GameInfo();
                Message.ReadFrom(reader);
            }
            else if (type == PacketType.ClientUpdate)
            {
                Message = new ClientUpdateMessage();
                Message.ReadFrom(reader);
            }
            else
            {
                throw new NetworkException();
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Lobby);

            if (Message is PlayerJoin)
            {
                writer.Write((byte)PacketType.PlayerJoin);
            }
            else if (Message is ClientJoin)
            {
                writer.Write((byte)PacketType.ClientJoin);
            }
            else if (Message is LobbyInfo)
            {
                writer.Write((byte)PacketType.LobbyInfo);
            }
            else if (Message is GameInfo)
            {
                writer.Write((byte)PacketType.GameInfo);
            }
            else if (Message is ClientUpdateMessage)
            {
                writer.Write((byte)PacketType.ClientUpdate);
            }
            else
            {
                throw new NetworkException();
            }

            Message.WriteTo(writer);
        }
    }
}
