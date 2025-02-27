// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using MrBoom.NetworkProtocol;

namespace MrBoom.Server
{
    public interface IGameLobby : IServerGameEntity
    {
        int StartIn { get; }

        ClientInfo ClientJoin(ClientJoinRequest request, IPEndPoint ipep);
        IEnumerable<ClientInfo> GetClients();
        IEnumerable<LobbyPlayer> GetPlayers();
        LobbyPlayer PlayerJoin(Guid id);
    }

    public class GameLobby : IGameLobby
    {
        private readonly List<ClientInfo> clients;
        private readonly List<LobbyPlayer> players;
        private readonly ILogger logger;
        private int index = 0;

        private int tick = 0;
        public int StartIn { get; private set; } = -1;

        public GameLobby(ILogger<GameLobby> logger)
        {
            players = new List<LobbyPlayer>();
            clients = new List<ClientInfo>();

            this.logger = logger;
        }

        public LobbyPlayer PlayerJoin(Guid id)
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer("qqq");

            lobbyPlayer.Id = id;
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

        public void ServerUpdate()
        {
            tick++;

            if (players.Count >= 2 && StartIn == -1)
            {
                StartIn = 600;
            }

            if (StartIn > 0)
            {
                StartIn--;
            }
        }
    }
}
