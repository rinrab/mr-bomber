// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server.MatchMaking
{
    public interface IMatchMakingLobbyInfo
    {
        Guid Key { get; }
        bool IsFull { get; }
    }
}
