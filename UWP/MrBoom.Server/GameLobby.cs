// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Net;
using System.Net.Sockets;
using MrBoom.Common;
using MrBoom.NetworkProtocol;

namespace MrBoom.Server
{
    public interface IGameLobby
    {
        ClientInfo ClientJoin(ClientJoinRequest request, IPEndPoint ipep);
        IEnumerable<ClientInfo> GetClients();
        LobbyInfo GetLobbyInfo();
        IEnumerable<LobbyPlayer> GetPlayers();
        PlayerInfo PlayerJoin(PlayerJoinInfo player);
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

        public PlayerInfo PlayerJoin(PlayerJoinInfo player)
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer(player.Name);

            lobbyPlayer.Id = Guid.NewGuid();
            lobbyPlayer.Index = index;

            players.Add(lobbyPlayer);
            index++;

            return lobbyPlayer.GetMe();
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

        public LobbyInfo GetLobbyInfo()
        {
            List<PlayerInfo> players = new List<PlayerInfo>();

            foreach (LobbyPlayer player in this.players)
            {
                players.Add(player.GetMe());
            }

            return new LobbyInfo
            {
                Players = players,
            };
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
