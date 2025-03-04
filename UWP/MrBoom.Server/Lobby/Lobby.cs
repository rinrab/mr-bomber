// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;
using MrBoom.Server.MatchMaking;

namespace MrBoom.Server.Lobby
{
    public class Lobby : ILobby, IMatchMakingLobbyInfo
    {
        private readonly List<ClientInfo> clients;
        private readonly List<LobbyPlayer> players;

        private readonly ILogger logger;
        private readonly IServiceProvider serviceProvider;

        private ILobbyState state;

        public IUdpServer UdpServer { get; }

        public Guid Key { get; }

        public bool IsFull => state is not LobbyJoinState;

        public Lobby(ILogger<Lobby> logger, IUdpServer udpServer, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            UdpServer = udpServer;
            this.serviceProvider = serviceProvider;
            clients = new List<ClientInfo>();
            players = new List<LobbyPlayer>();

            Key = Guid.NewGuid();

            state = ActivatorUtilities.CreateInstance<LobbyJoinState>(serviceProvider, this);
        }

        public void AddClient(ClientInfo client)
        {
            clients.Add(client);
        }

        public void AddPlayer(LobbyPlayer player)
        {
            players.Add(player);
        }

        public ClientInfo? GetClient(Guid id)
        {
            return clients.FirstOrDefault(item => item.ClientSecret == id);
        }

        public IEnumerable<ClientInfo> GetClients()
        {
            foreach (ClientInfo client in clients)
            {
                if (!client.IsFrozen)
                {
                    yield return client;
                }
            }
        }

        public int GetPlayerCount()
        {
            return players.Count;
        }

        public IEnumerable<LobbyPlayer> GetPlayers()
        {
            return players;
        }

        public void FilterDeadClients()
        {
            clients.RemoveAll(item =>
            {
                if (item.IsDead)
                {
                    logger.LogWarning("Client {id} on {ip} died; connection timed out", item.CorishInfo, item.IpAddress);
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        public void OnMessageReceived(IMessage message, Guid clientSecret, IPEndPoint endPoint)
        {
            state.OnMessageReceived(message, clientSecret, endPoint);
        }

        public async Task SendPackets(IUdpServer udpServer, CancellationToken stoppingToken)
        {
            await state.SendPackets(udpServer, stoppingToken);
        }

        public void ServerUpdate()
        {
            state.ServerUpdate();
        }

        public void SetState(ILobbyState state)
        {
            this.state = state;
        }

        public void SetState<T>() where T : ILobbyState
        {
            SetState(ActivatorUtilities.CreateInstance<T>(serviceProvider, this));
        }

        public ILobbyState GetState()
        {
            return state;
        }

        public async Task SendPacket(Packet packet, IPEndPoint ipAddress, CancellationToken cancellationToken)
        {
            await UdpServer.SendPacket(packet, ipAddress, cancellationToken);
        }
    }
}
