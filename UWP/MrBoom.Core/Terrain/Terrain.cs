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
    public class Terrain : IServerGameEntity
    {
        public readonly IRandom Random;

        public readonly Map mapData;
        public readonly TerrainStartInfo startInfo;
        public readonly PowerUpProvider powerUpProvider;
        public readonly SpawnProvider spawns;
        public readonly TerrainMap map;
        public readonly TerrainFinal final;
        public readonly TerrainSpriteHost sprites;
        public readonly TerrainTimer timer;
        public readonly GameEndedHandler gameEndedHandler;
        public readonly TerrainAIInfoProvider aiInfoProvider;
        public readonly TerrainProxyProvider proxy;

        public GameResult Result => gameEndedHandler.Result;
        public int Winner => gameEndedHandler.Winner;

        public Terrain(int levelIndex, IRandom random)
        {
            Random = random;
            mapData = MapData.Data[levelIndex];

            startInfo = new TerrainStartInfo(levelIndex);
            powerUpProvider = new PowerUpProvider(random, mapData);
            spawns = new SpawnProvider(random);
            map = new TerrainMap(mapData, spawns, powerUpProvider);
            timer = new TerrainTimer();
            final = new TerrainFinal(map, mapData, random, timer);
            sprites = new TerrainSpriteHost(spawns, random, mapData, this);
            gameEndedHandler = new GameEndedHandler(timer, sprites, final);
            aiInfoProvider = new TerrainAIInfoProvider(map, sprites);
            proxy = new TerrainProxyProvider(map, final, sprites, timer, startInfo);
        }

        public void ServerUpdate()
        {
            map.ServerUpdate();
            final.ServerUpdate();
            sprites.ServerUpdate();
            timer.ServerUpdate();
            gameEndedHandler.ServerUpdate();
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
    }
}
