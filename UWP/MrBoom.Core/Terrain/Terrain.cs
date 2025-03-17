// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrBoom.Common;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;

namespace MrBoom
{
    public interface ITerrain
    {
        int Width { get; }
        int Height { get; }
        int LevelIndex { get; }

        Cell GetCell(int x, int y);
        void SetCell(int x, int y, Cell cell);

        int TimeLeft { get; }
        int ApocalypseSpeed { get; }
        int MaxApocalypse { get; }

        int GetCellApocalypseRemainingTime(int cellX, int cellY);

        bool IsWalkable(int x, int y);
        void PutBomb(int cellX, int cellY, int maxBoom, bool rcAllowed, IBombOwner owner);
        void BurnCell(int cellX, int cellY);
        void DitonateBomb(int bombX, int bombY);
        Cell GeneratePowerUp(PowerUpType powerUpType);

        bool IsTouchingMonster(int cellX, int cellY);
        bool IsMonsterComing(int cellX, int cellY);
        int GetKillablePlayers(int cellX, int cellY);

        IEnumerable<SpriteBase> GetSprites();
        IEnumerable<ServerPlayer> GetPlayers();
        IEnumerable<AbstractMonster> GetMonsters();
    }

    public class Terrain : ITerrain, ITerrainProxy
    {
        public int Width { get; }
        public int Height { get; }
        public int LevelIndex { get; }
        public int TimeLeft { get; private set; }

        public SoundEffectType SoundsToPlay;

        public GameResult Result => gameEndedHandler.Result;
        public int Winner => gameEndedHandler.Winner;

        public int ApocalypseSpeed => timer.ApocalypseSpeed;
        public int MaxApocalypse => final.MaxApocalypse;

        public IList<ISpriteProxy> Sprites => sprites.GetSprites().Select(sprite => sprite.GetService<ISpriteProxy>()).ToList();

        public readonly IRandom Random;

        private readonly Map mapData;
        private readonly PowerUpProvider powerUpProvider;
        private readonly SpawnProvider spawns;
        private readonly TerrainMap map;
        private readonly TerrainFinal final;
        private readonly TerrainSpriteHost sprites;
        private readonly TerrainTimer timer;
        private readonly GameEndedHandler gameEndedHandler;
        private readonly TerrainAIInfoProvider aiInfoProvider;

        public Terrain(int levelIndex, IRandom random)
        {
            LevelIndex = levelIndex;
            Random = random;
            mapData = MapData.Data[levelIndex];

            Width = mapData.Data[0].Length;
            Height = mapData.Data.Length;
            TimeLeft = (mapData.Time + 31) * 60;

            powerUpProvider = new PowerUpProvider(random, mapData);
            spawns = new SpawnProvider(random);
            map = new TerrainMap(mapData, spawns, powerUpProvider);
            timer = new TerrainTimer();
            final = new TerrainFinal(map, mapData, random, timer);
            sprites = new TerrainSpriteHost(spawns, random, mapData, this);
            gameEndedHandler = new GameEndedHandler(timer, sprites, final);
            aiInfoProvider = new TerrainAIInfoProvider(map, sprites);
        }

        public void AddPlayer(ServerPlayer player)
        {
            sprites.AddPlayer(player);
        }

        public void InitializeMonsters()
        {
            sprites.InitializeMonsters();
        }

        public int GetCellApocalypseRemainingTime(int cellX, int cellY)
        {
            return final.GetCellApocalypseRemainingTime(cellX, cellY);
        }

        public void Update()
        {
            map.ServerUpdate();
            final.ServerUpdate();
            sprites.ServerUpdate();
            timer.ServerUpdate();
            gameEndedHandler.ServerUpdate();
        }

        public void ClientUpdate()
        {
        }

        public Cell GetCell(int x, int y)
        {
            return map[x, y];
        }

        public void SetCell(int x, int y, Cell cell)
        {
            map[x, y] = cell;
        }

        public bool IsWalkable(int x, int y)
        {
            return map.IsWalkable(x, y);
        }

        public void DitonateBomb(int bombX, int bombY)
        {
            map.DitonateBomb(bombX, bombY);
        }

        public Cell GeneratePowerUp(PowerUpType powerUpType)
        {
            return powerUpProvider.GeneratePowerUp(powerUpType);
        }

        public void PutBomb(int cellX, int cellY, int maxBoom, bool rcAllowed, IBombOwner owner)
        {
            map.PutBomb(cellX, cellY, maxBoom, rcAllowed, owner);
        }

        Cell GenerateGiven()
        {
            return powerUpProvider.GenerateGiven();
        }

        public void PlaySound(SoundEffectType sound)
        {
            SoundsToPlay |= sound;
        }

        public bool IsTouchingMonster(int cellX, int cellY)
        {
            return aiInfoProvider.IsTouchingMonster(cellX, cellY);
        }

        public bool IsMonsterComing(int cellX, int cellY)
        {
            return aiInfoProvider.IsMonsterComing(cellX, cellY);
        }

        public int GetKillablePlayers(int cellX, int cellY)
        {
            return aiInfoProvider.GetKillablePlayers(cellX, cellY);
        }

        public IEnumerable<SpriteBase> GetSprites()
        {
            return sprites.GetSprites();
        }

        public IEnumerable<ServerPlayer> GetPlayers()
        {
            return sprites.GetPlayers();
        }

        public IEnumerable<AbstractMonster> GetMonsters()
        {
            return sprites.GetMonsters();
        }

        public string GetCellDebugInfo(int cellX, int cellY)
        {
            List<string> list = new List<string>();

            foreach (ServerPlayer sprite in sprites.GetPlayers())
            {
                if (sprite.GetService<SpriteHealthController>().IsAlive)
                {
                    string debugInfo = sprite.GetCellDebugInfo(cellX, cellY);

                    if (!string.IsNullOrEmpty(debugInfo) && !list.Contains(debugInfo))
                    {
                        list.Add(debugInfo);
                    }
                }
            }

            return string.Join('\n', list);
        }

        public string GetDebugInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"DEBUG INFO");
            sb.AppendLine($"Version: {Version.VersionString}");

            //foreach (Sprite sprite in GetSprites())
            //{
            //    sb.AppendLine(sprite.GetDebugInfo());
            //}

            sb.AppendLine($"F1 - detonate all");
            sb.AppendLine($"F2 - clear all");
            sb.AppendLine($"F3 - apocalypse");
            sb.AppendLine($"F4 - toggle debug info");
            sb.AppendLine($"F5 - give all");

            return sb.ToString();
        }

        public void DetonateAll(bool generateBonus)
        {
            for (int i = 0; i < map.CellCount; i++)
            {
                if (map[i].Type == TerrainType.TemporaryWall)
                {
                    Cell next = generateBonus ? GenerateGiven() : new Cell(TerrainType.Free);

                    map[i] = new Cell(TerrainType.PermanentWall)
                    {
                        Index = 0,
                        animateDelay = 4,
                        Next = next
                    };
                }
            }
        }

        public void StartApocalypse()
        {
            final.StartApocalypse();
        }

        public void GiveAll()
        {
            foreach (ServerPlayer player in sprites.GetPlayers())
            {
                player.GiveAll();
            }
        }

        public void BurnCell(int cellX, int cellY)
        {
            SetCell(cellX, cellY, new Cell(TerrainType.PowerUpFire)
            {
                Index = 0,
                animateDelay = 6,
                Next = new Cell(TerrainType.Free)
            });

            PlaySound(SoundEffectType.Sac);
        }
    }
}
