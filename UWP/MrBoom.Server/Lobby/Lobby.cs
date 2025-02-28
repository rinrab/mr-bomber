// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server.Lobby
{
    public interface ILobby
    {
        IEnumerable<ClientInfo> GetClients();
        IEnumerable<LobbyPlayer> GetPlayers();

        int GetPlayerCount();

        void AddClient(ClientInfo client);
        void AddPlayer(LobbyPlayer player);
    }

    public class Lobby : ILobby
    {
        private readonly List<ClientInfo> clients;
        private readonly List<LobbyPlayer> players;

        public Lobby()
        {
            clients = new List<ClientInfo>();
            players = new List<LobbyPlayer>();
        }

        public void AddClient(ClientInfo client)
        {
            clients.Add(client);
        }

        public void AddPlayer(LobbyPlayer player)
        {
            players.Add(player);
        }

        public IEnumerable<ClientInfo> GetClients()
        {
            return clients;
        }

        public int GetPlayerCount()
        {
            return players.Count;
        }

        public IEnumerable<LobbyPlayer> GetPlayers()
        {
            return players;
        }
    }
}
