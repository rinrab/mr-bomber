// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;

namespace MrBoom.Server
{
    public interface IGameLobby
    {
        LobbyInfo GetLobbyInfo();
        PlayerInfo PlayerJoin(PlayerJoinInfo player);
    }

    public class GameLobby : IGameLobby
    {
        private readonly List<LobbyPlayer> players;
        private int index = 0;

        public GameLobby()
        {
            players = new List<LobbyPlayer>();
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
    }
}
