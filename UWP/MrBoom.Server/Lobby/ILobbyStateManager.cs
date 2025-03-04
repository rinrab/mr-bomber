// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Server.Lobby
{
    public interface ILobbyStateManager
    {
        void SetState(ILobbyState state);
        void SetState<T>() where T : ILobbyState;
        ILobbyState GetState();
    }
}
