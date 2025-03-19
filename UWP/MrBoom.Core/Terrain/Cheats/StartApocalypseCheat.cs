// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;

namespace MrBoom.Core.Terrain.Cheats
{
    public class StartApocalypseCheat : IServerCheatModule
    {
        private readonly TerrainFinal final;

        public StartApocalypseCheat(TerrainFinal final)
        {
            this.final = final;
        }

        public void ApplyCheat()
        {
            final.StartApocalypse();
        }

        public int GetCheatCode() => 2;

        public string GetCheatName() => "Force Apocalypse";
    }
}
