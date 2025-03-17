// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.Common;

namespace MrBoom.Core.Terrain
{
    public class PowerUpProvider
    {
        private readonly IRandom random;

        private readonly List<PowerUpType> powerUpList;

        public PowerUpProvider(IRandom random, Map mapData)
        {
            this.random = random;

            powerUpList = new List<PowerUpType>();

            foreach (var bonus in mapData.PowerUps)
            {
                for (int i = 0; i < bonus.Count; i++)
                {
                    powerUpList.Add(bonus.Type);
                }
            }
        }

        public Cell GenerateGiven()
        {
            int rnd = random.Next(int.MaxValue);
            if (rnd < int.MaxValue / 2)
            {
                var powerUpType = random.NextElement(powerUpList);

                return GeneratePowerUp(powerUpType);
            }
            else
            {
                return new Cell(TerrainType.Free);
            }
        }

        public Cell GeneratePowerUp(PowerUpType powerUpType)
        {
            return new Cell(TerrainType.PowerUp)
            {
                Index = 0,
                animateDelay = 8,
                PowerUpType = powerUpType
            };
        }
    }
}
