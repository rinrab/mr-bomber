// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Terrain.Cheats
{
    public abstract class DetonateAllBase : IServerCheatModule
    {
        private readonly TerrainMap map;
        private readonly PowerUpProvider powerUpProvider;
        private readonly bool generateBonus;

        public DetonateAllBase(TerrainMap map, PowerUpProvider powerUpProvider, bool generateBonus)
        {
            this.map = map;
            this.powerUpProvider = powerUpProvider;
            this.generateBonus = generateBonus;
        }

        public void ApplyCheat()
        {
            for (int i = 0; i < map.CellCount; i++)
            {
                if (map[i].Type == TerrainType.TemporaryWall)
                {
                    Cell next = generateBonus ? powerUpProvider.GenerateGiven() : new Cell(TerrainType.Free);

                    map[i] = new Cell(TerrainType.PermanentWall)
                    {
                        Index = 0,
                        animateDelay = 4,
                        Next = next
                    };
                }
            }
        }

        public abstract int GetCheatCode();
        public abstract string GetCheatName();
    }

    public class DetonateAll : DetonateAllBase
    {
        public DetonateAll(TerrainMap map, PowerUpProvider powerUpProvider) : base(map, powerUpProvider, true)
        {
        }

        public override int GetCheatCode() => 0;

        public override string GetCheatName() => "Detonate all";
    }

    public class ClearAll : DetonateAllBase
    {
        public ClearAll(TerrainMap map, PowerUpProvider powerUpProvider) : base(map, powerUpProvider, false)
        {
        }

        public override int GetCheatCode() => 1;

        public override string GetCheatName() => "Clear all";
    }
}
