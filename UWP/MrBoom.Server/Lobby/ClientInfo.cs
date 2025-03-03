// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.Core;
using MrBoom.NetworkProtocol.Proxy;

namespace MrBoom.Server.Lobby
{
    public class ClientInfo
    {
        public Guid ClientSecret { get; set; }

        public IClientInfo CorishInfo => new ClientInfoGuid(ClientSecret);

        public IPEndPoint IpAddress { get; set; }

        public DateTime LastPacketReceivedTime { get; private set; }

        public bool IsFrozen => DateTime.UtcNow - LastPacketReceivedTime > TimeSpan.FromMilliseconds(500);

        public bool IsDead => DateTime.UtcNow - LastPacketReceivedTime > TimeSpan.FromSeconds(5);

        public ClientInfo(IPEndPoint ipAddress, Guid clientSecret)
        {
            IpAddress = ipAddress;
            ClientSecret = clientSecret;
            OnPacketReceived();
        }

        public void OnPacketReceived()
        {
            LastPacketReceivedTime = DateTime.UtcNow;
        }
    }
}
