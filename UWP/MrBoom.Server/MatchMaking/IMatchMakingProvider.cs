// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server.MatchMaking
{
    public interface IMatchMakingProvider
    {
        Guid AssignLobby();
    }
}
