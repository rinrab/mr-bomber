// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol.Messages;

namespace MrBoom.Server.Lobby
{
    public interface ILobby : ILobbyState, ILobbyStateManager
    {
        IEnumerable<ClientInfo> GetClients();
        IEnumerable<LobbyPlayer> GetPlayers();

        int GetPlayerCount();

        void AddClient(ClientInfo client);
        void AddPlayer(LobbyPlayer player);

        ClientInfo? GetClient(Guid id);
        void FilterDeadClients();
    }

    public class Lobby : ILobby
    {
        private readonly List<ClientInfo> clients;
        private readonly List<LobbyPlayer> players;

        private readonly ILogger logger;

        private LobbyStateHolder state { get; }

        public Lobby(ILogger logger)
        {
            this.logger = logger;

            clients = new List<ClientInfo>();
            players = new List<LobbyPlayer>();

            state = new LobbyStateHolder();
            state.SetState(new LobbyJoinState(this, logger));
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
            this.state.SetState(state);
        }

        public ILobbyState GetState()
        {
            return state.GetState();
        }
    }
}
