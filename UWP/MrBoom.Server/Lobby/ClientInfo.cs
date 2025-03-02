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
    }
}
