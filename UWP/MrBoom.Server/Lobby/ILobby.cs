// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public interface ILobby : ILobbyState, ILobbyStateManager
    {
        IUdpServer UdpServer { get; }

        IEnumerable<ClientInfo> GetClients();
        IEnumerable<LobbyPlayer> GetPlayers();

        int GetPlayerCount();

        void AddClient(ClientInfo client);
        void AddPlayer(LobbyPlayer player);

        ClientInfo? GetClient(Guid id);
        LobbyPlayer? GetPlayer(Guid id);

        void FilterDeadClients();

        Task SendPacket(Packet packet, IPEndPoint ipAddress, CancellationToken cancellationToken);
    }
}
