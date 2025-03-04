// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server.Lobby
{
    public interface ILobbyProvider
    {
        Guid CreateLobby();
        Guid AssignLobby();
    }
}
