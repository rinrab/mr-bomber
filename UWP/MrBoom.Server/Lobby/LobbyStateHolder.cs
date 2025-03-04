// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public class LobbyStateHolder : ILobbyState, ILobbyStateManager
    {
        private ILobbyState? current;

        public void SetState(ILobbyState state)
        {
            current = state;
        }

        public ILobbyState GetState()
        {
            return current;
        }

        public void OnMessageReceived(IMessage message, Guid clientSecret, IPEndPoint endPoint)
        {
            current?.OnMessageReceived(message, clientSecret, endPoint);
        }

        public void ServerUpdate()
        {
            current?.ServerUpdate();
        }

        public async Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken)
        {
            if (current != null)
            {
                await current.SendPackets(udpServer, stoppingToken);
            }
        }
    }
}
