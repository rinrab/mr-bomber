// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MrBoom.Common;
using MrBoom.Core;
using MrBoom.Core.Sprites;
using MrBoom.Core.Terrain;
using MrBoom.Core.Terrain.Cheats;

namespace MrBoom
{
    public class Terrain : GameEntityBase
    {
        public Terrain(int levelIndex, IRandom random)
        {
            AddSingleton(random);
            AddSingleton(MapData.Data[levelIndex]);
            AddSingleton(new TerrainStartInfo(levelIndex));

            AddSingleton<TerrainInitialMapProvider>();
            AddSingleton<SpawnProvider>();
            AddSingleton<TerrainMap>();
            AddSingleton<TerrainTimer>();
            AddSingleton<TerrainFinal>();

            AddSingleton<PowerUpProvider>();
            AddSingleton<TerrainSpriteHost>();

            AddSingleton<GameEndedHandler>();
            AddSingleton<TerrainAIInfoProvider>();
            AddSingleton<TerrainProxyProvider>();

            AddSingleton<CheatHost>();
        }

        public override void ServerUpdate()
        {
            base.ServerUpdate();
        }

        public string GetCellDebugInfo(int cellX, int cellY)
        {
            //List<string> list = new List<string>();

            //foreach (ServerPlayer sprite in sprites.GetPlayers())
            //{
            //    if (sprite.GetService<SpriteHealthController>().IsAlive)
            //    {
            //        string debugInfo = sprite.GetCellDebugInfo(cellX, cellY);

            //        if (!string.IsNullOrEmpty(debugInfo) && !list.Contains(debugInfo))
            //        {
            //            list.Add(debugInfo);
            //        }
            //    }
            //}

            //return string.Join('\n', list);

            return string.Empty;
        }

        public string GetDebugInfo()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"DEBUG INFO");
            sb.AppendLine($"Version: {Version.VersionString}");

            sb.AppendLine();

            foreach (GameEntityBase sprite in GetService<TerrainSpriteHost>().GetSprites())
            {
                foreach (var debugInfo in sprite.EnumerateServices<IDebugInfoProvider>())
                {
                    sb.AppendLine(debugInfo.GetDebugInfo());
                }
            }

            sb.AppendLine();

            sb.AppendLine($"F1 - detonate all");
            sb.AppendLine($"F2 - clear all");
            sb.AppendLine($"F3 - apocalypse");
            sb.AppendLine($"F4 - toggle debug info");
            sb.AppendLine($"F5 - give all");

            return sb.ToString();
        }
    }
}
