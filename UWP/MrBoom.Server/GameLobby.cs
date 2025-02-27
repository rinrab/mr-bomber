// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol;

namespace MrBoom.Server
{
    public interface IGameLobby
    {
        ClientInfo ClientJoin(ClientJoinRequest request, IPEndPoint ipep);
        IEnumerable<ClientInfo> GetClients();
        IEnumerable<LobbyPlayer> GetPlayers();
        LobbyPlayer PlayerJoin();
    }

    public class GameLobby : IGameLobby
    {
        private readonly List<ClientInfo> clients;
        private readonly List<LobbyPlayer> players;
        private readonly ILogger logger;
        private int index = 0;

        public GameLobby(ILogger<GameLobby> logger)
        {
            players = new List<LobbyPlayer>();
            clients = new List<ClientInfo>();

            this.logger = logger;
        }

        public LobbyPlayer PlayerJoin()
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer("qqq");

            lobbyPlayer.Id = Guid.NewGuid();
            lobbyPlayer.Index = index;

            players.Add(lobbyPlayer);
            index++;

            return lobbyPlayer;
        }

        public ClientInfo ClientJoin(ClientJoinRequest request, IPEndPoint ipep)
        {
            ClientInfo clientInfo = new ClientInfo()
            {
                ClientSecret = Guid.NewGuid(),
                IpAddress = ipep,
            };

            clients.Add(clientInfo);

            return clientInfo;
        }

        public IEnumerable<ClientInfo> GetClients()
        {
            return clients;
        }

        public IEnumerable<LobbyPlayer> GetPlayers()
        {
            return players;
        }
    }
}
