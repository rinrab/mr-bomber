// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Text;
using MrBoom.Core;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public class ClientInfo
    {
        public ILobby Lobby { get; }

        public Guid ClientSecret { get; set; }

        public IClientInfo CorishInfo => new ClientInfoGuid(ClientSecret);

        public IPEndPoint IpAddress { get; set; }

        public DateTime LastPacketReceivedTime { get; private set; }

        public bool IsFrozen => DateTime.UtcNow - LastPacketReceivedTime > TimeSpan.FromMilliseconds(500);

        public bool IsDead => DateTime.UtcNow - LastPacketReceivedTime > TimeSpan.FromSeconds(5);

        public ClientInfo(ILobby lobby, IPEndPoint ipAddress, Guid clientSecret)
        {
            Lobby = lobby;
            IpAddress = ipAddress;
            ClientSecret = clientSecret;
            OnPacketReceived();
        }

        public void OnPacketReceived()
        {
            LastPacketReceivedTime = DateTime.UtcNow;
        }

        public async Task SendMessage(IMessage message, CancellationToken cancellationToken)
        {
            await Lobby.SendPacket(new Packet(message), IpAddress, cancellationToken);
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            result.Append($"{CorishInfo}: {IpAddress}");

            if (IsFrozen)
            {
                result.Append(", frozen");
            }

            if (IsDead)
            {
                result.Append(", dead");
            }

            return result.ToString();
        }
    }
}
