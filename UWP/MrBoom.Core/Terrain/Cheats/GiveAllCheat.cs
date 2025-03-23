// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites;

namespace MrBoom.Core.Terrain.Cheats
{
    public class GiveAllCheat : IServerCheatModule
    {
        private readonly TerrainSpriteHost sprites;

        public GiveAllCheat(TerrainSpriteHost sprites)
        {
            this.sprites = sprites;
        }

        public void ApplyCheat()
        {
            foreach (ServerPlayer player in sprites.GetPlayers())
            {
                player.GiveAll();
            }
        }

        public int GetCheatCode() => 4;

        public string GetCheatName() => "Give all";
    }
}
