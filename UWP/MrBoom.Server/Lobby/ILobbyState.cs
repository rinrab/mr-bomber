// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.Core;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public interface ILobbyState : IServerGameEntity
    {
        void OnMessageReceived(IMessage message, Guid clientSecret, IPEndPoint endPoint);
        Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken);
    }
}
