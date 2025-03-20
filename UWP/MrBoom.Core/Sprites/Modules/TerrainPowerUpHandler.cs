// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Modules
{
    public class TerrainPowerUpHandler : IPowerUpHandler
    {
        private readonly TerrainMap map;
        private readonly TerrainTimer timer;
        private readonly TerrainFinal final;

        public TerrainPowerUpHandler(TerrainMap map, TerrainTimer timer, TerrainFinal final)
        {
            this.map = map;
            this.timer = timer;
            this.final = final;
        }

        public PowerUpPickResult PickPowerUp(PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.Banana)
            {
                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        if (map.GetCell(x, y).Type == TerrainType.Bomb)
                        {
                            map.DitonateBomb(x, y);
                        }
                    }
                }

                return PowerUpPickResult.Pick;
            }
            else if (powerUpType == PowerUpType.Clock)
            {
                if (timer.TimeLeft > 31 * 60 + final.MaxApocalypse * timer.ApocalypseSpeed)
                {
                    // TODO: terrain.TimeLeft += 60 * 60;
                    // PlaySound(SoundEffectType.Clock);
                    return PowerUpPickResult.Pick;
                }
                else
                {
                    return PowerUpPickResult.Burn;
                }
            }
            else
            {
                return PowerUpPickResult.Skip;
            }
        }
    }
}
