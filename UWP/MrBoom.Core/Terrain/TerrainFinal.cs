// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using MrBoom.Common;

namespace MrBoom.Core.Terrain
{
    public class TerrainFinal : IServerGameEntity
    {
        private readonly TerrainMap map;
        private readonly Map mapData;
        private readonly IRandom random;
        private readonly TerrainTimer timer;
        private readonly ISoundController soundController;
        private readonly Grid<byte> final;

        private int lastApocalypseSound = -1;

        public int MaxApocalypse { get; private set; }

        public TerrainFinal(TerrainMap map,
                            Map mapData,
                            IRandom random,
                            TerrainTimer timer,
                            ISoundController soundController)
        {
            this.map = map;
            this.mapData = mapData;
            this.random = random;
            this.timer = timer;
            this.soundController = soundController;

            final = new Grid<byte>(map.Width, map.Height);

            for (int i = 0; i < final.CellCount; i++)
            {
                byte fin = mapData.Final[i];
                final[i] = fin;
                if (fin != 255)
                {
                    MaxApocalypse = Math.Max(fin, MaxApocalypse);
                }
            }
        }

        public void ServerUpdate()
        {
            int index = (30 * 60 - timer.TimeLeft) / timer.ApocalypseSpeed;
            for (int i = 0; i < final.CellCount; i++)
            {
                Cell cell = map[i];
                if (index == MaxApocalypse + 5)
                {
                    // Blow cell if final index is 255
                    if (cell.Type == TerrainType.TemporaryWall)
                    {
                        map[i] = new Cell(TerrainType.PowerUpFire)
                        {
                            Index = 0,
                            Next = new Cell(TerrainType.Free)
                        };
                        soundController.PlaySound(SoundEffectType.Sac);
                    }
                }
                else if (final[i] == index && final[i] != 255)
                {
                    // Replace cell with apocalypse cell
                    if (cell.Type == TerrainType.Bomb)
                    {
                        cell.owner.OnBombReplaced(i % map.Width, i / map.Width);
                    }
                    if (cell.Type != TerrainType.PermanentWall)
                    {
                        map[i] = new Cell(TerrainType.Apocalypse)
                        {
                            Index = 0,
                            Next = new Cell(TerrainType.Apocalypse),
                        };
                        if (Math.Abs(lastApocalypseSound - timer.TimeLeft) > 60)
                        {
                            soundController.PlaySound(SoundEffectType.Sac);
                            lastApocalypseSound = timer.TimeLeft;
                        }
                    }
                }
            }

            if (mapData.IsBombApocalypse && index > 0)
            {
                if (timer.TimeLeft % 16 == 0)
                {
                    Directions dir = random.NextElement(DirectionsExtensions.Horizontal());
                    int x = (dir == Directions.Right) ? 1 : map.Width - 2;
                    int y = (random.Next(0, map.Height / 2)) * 2 + 1;

                    map.PutBomb(x, y, 4, false, null);
                    map[x, y].DeltaX = dir.DeltaX() * 2;
                }
            }
        }

        public int GetCellApocalypseRemainingTime(int cellX, int cellY)
        {
            byte apocalypseIndex = final[cellX, cellY];
            if (apocalypseIndex == 255)
            {
                return int.MaxValue;
            }
            else
            {
                return apocalypseIndex * timer.ApocalypseSpeed + timer.TimeLeft - 30 * 60;
            }
        }

        public void StartApocalypse()
        {
            const int timeToApocalypse = (30 + 2) * 60;

            if (timer.TimeLeft > timeToApocalypse)
            {
                timer.TimeLeft = timeToApocalypse;
            }
        }
    }
}
