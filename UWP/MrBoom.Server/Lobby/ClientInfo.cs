// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;

namespace MrBoom.Server.Lobby
{
    public class ClientInfo
    {
        public Guid ClientSecret { get; set; }

        public IPEndPoint IpAddress { get; set; }
    }
}
