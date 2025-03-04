// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server.MatchMaking
{
    public interface ILobbyProvider
    {
        Guid CreateLobby();
        IEnumerable<IMatchMakingLobbyInfo> EnumerateLobbies();
    }
}
