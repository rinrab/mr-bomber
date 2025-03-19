// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Terrain.Cheats
{
    public interface IServerCheatModule
    {
        int GetCheatCode();
        string GetCheatName();

        void ApplyCheat();
    }
}
