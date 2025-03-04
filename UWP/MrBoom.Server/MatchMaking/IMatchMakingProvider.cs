// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server.MatchMaking
{
    public interface IMatchMakingProvider
    {
        Task<Guid> AssignLobbyAsync(CancellationToken cancellationToken);
    }
}
