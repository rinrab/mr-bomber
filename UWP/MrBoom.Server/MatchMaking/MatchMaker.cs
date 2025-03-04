// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server.MatchMaking
{
    public class MatchMaker : IMatchMakingProvider
    {
        private readonly ILobbyProvider lobbyProvider;

        public MatchMaker(ILobbyProvider lobbyProvider)
        {
            this.lobbyProvider = lobbyProvider;
        }

        public Guid AssignLobby()
        {
            foreach (var lobby in lobbyProvider.EnumerateLobbies())
            {
                if (!lobby.IsFull)
                {
                    return lobby.Key;
                }
            }

            return lobbyProvider.CreateLobby();
        }
    }
}
