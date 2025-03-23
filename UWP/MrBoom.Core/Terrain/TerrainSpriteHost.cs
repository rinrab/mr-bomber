// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.Common;
using MrBoom.Core.Service;
using MrBoom.Core.Sprites;
using MrBoom.Core.Sprites.Controllers;

namespace MrBoom.Core.Terrain
{
    public class TerrainSpriteHost : IServerGameEntity
    {
        private readonly List<ServerPlayer> players;
        private readonly List<AbstractMonster> monsters;

        private readonly SpawnProvider spawns;
        private readonly TerrainMap map;
        private readonly IRandom random;
        private readonly Map mapData;
        private readonly IBomberServiceProvider serviceProvider;

        public TerrainSpriteHost(SpawnProvider spawns,
                                 TerrainMap map,
                                 IRandom random,
                                 Map mapData,
                                 IBomberServiceProvider serviceProvider)
        {
            monsters = new List<AbstractMonster>();
            players = new List<ServerPlayer>();

            this.spawns = spawns;
            this.map = map;
            this.random = random;
            this.mapData = mapData;
            this.serviceProvider = serviceProvider;
        }

        public void ServerUpdate()
        {
            foreach (GameEntityBase sprite in GetSprites())
            {
                sprite.ServerUpdate();

                // PlaySound(sprite.SoundsToPlay);
            }

            foreach (GameEntityBase sprite1 in GetSprites())
            {
                foreach (GameEntityBase sprite2 in GetSprites())
                {
                    var position1 = sprite1.GetService<SpritePosition>();
                    var position2 = sprite2.GetService<SpritePosition>();
                    var effect1 = sprite1.GetService<SpriteEffectController>();
                    var effect2 = sprite2.GetService<SpriteEffectController>();
                    var health1 = sprite1.GetService<SpriteHealthController>();
                    var health2 = sprite2.GetService<SpriteHealthController>();

                    if (position1.CellX == position2.CellX &&
                        position1.CellY == position2.CellY &&
                        effect1.Skull.HasValue &&
                        !effect2.Skull.HasValue)
                    {
                        if (health1.IsAlive && health2.IsAlive)
                        {
                            effect2.SetSkull(effect1.Skull.Value);
                        }
                    }
                }
            }
        }

        public void AddPlayer(ServerPlayer player)
        {
            CellCoord spawn = spawns.GenerateSpawn().Value;

            player.AddServiceProvider(serviceProvider);
            player.GetService<SpritePosition>().MoveTo(spawn.X * 16, spawn.Y * 16);

            players.Add(player);
        }

        public void InitializeMonsters()
        {
            while (true)
            {
                var spawn = spawns.GenerateSpawn();
                if (!spawn.HasValue)
                {
                    break;
                }

                var data = random.NextElement(mapData.Monsters);

                AbstractMonster monster = data.GetMonster(spawn.Value.X * 16, spawn.Value.Y * 16);

                monster.AddServiceProvider(serviceProvider);

                monsters.Add(monster);
            }
        }

        public IEnumerable<GameEntityBase> GetSprites()
        {
            foreach (GameEntityBase sprite in players)
            {
                yield return sprite;
            }

            foreach (GameEntityBase sprite in monsters)
            {
                yield return sprite;
            }
        }

        public List<ServerPlayer> GetPlayers()
        {
            return players;
        }

        public IEnumerable<AbstractMonster> GetMonsters()
        {
            return monsters;
        }
    }
}
