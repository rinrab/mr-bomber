// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Server.Lobby;

namespace MrBoom.Server.Admin
{
    public interface IAdminLobbyProvider
    {
        IEnumerable<ILobby> EnumerateLobbies();
        ILobby GetLobby(Guid key);
    }
}
