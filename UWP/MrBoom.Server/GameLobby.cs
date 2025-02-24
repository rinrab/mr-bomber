// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;

namespace MrBoom.Server
{
    public interface IGameLobby
    {
        PlayerInfo JoinPlayer(PlayerJoinInfo player);
    }

    public class GameLobby : IGameLobby
    {
        private readonly List<LobbyPlayer> players;

        public GameLobby()
        {
            players = new List<LobbyPlayer>();
        }

        public PlayerInfo JoinPlayer(PlayerJoinInfo player)
        {
            LobbyPlayer lobbyPlayer = new LobbyPlayer(player.Name);

            lobbyPlayer.Id = Guid.NewGuid();

            players.Add(lobbyPlayer);

            return lobbyPlayer.GetMe();
        }
    }
}
