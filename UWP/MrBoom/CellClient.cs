// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrBoom
{
    public static class CellClient
    {
        public static AnimatedImage GetImages(this Cell cell, Assets assets, Assets.Level levelAssets)
        {
            if (cell.Type == TerrainType.TemporaryWall)
            {
                return levelAssets.Walls;
            }
            else if (cell.Type == TerrainType.PermanentWallTextured)
            {
                return levelAssets.PermanentWalls;
            }
            else if (cell.Type == TerrainType.Bomb)
            {
                return assets.Bomb;
            }
            else if (cell.Type == TerrainType.PowerUp)
            {
                return assets.PowerUps[(int)cell.PowerUpType];
            }
            else if (cell.Type == TerrainType.PowerUpFire)
            {
                return assets.Fire;
            }
            else if (cell.Type == TerrainType.Fire)
            {
                return assets.Flames[(int)cell.FlameDirection];
            }
            else
            {
                return null;
            }
        }
    }
}
