// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using MrBoom.Core.Sprites;

namespace MrBoom.Core.Terrain
{
    public class GameEndedHandler : IServerGameEntity
    {
        private readonly TerrainTimer timer;
        private readonly TerrainSpriteHost sprites;
        private readonly TerrainFinal final;

        public int Winner { get; private set; }
        public GameResult Result = GameResult.None;

        public GameEndedHandler(TerrainTimer timer, TerrainSpriteHost sprites, TerrainFinal final)
        {
            this.timer = timer;
            this.sprites = sprites;
            this.final = final;
        }

        public void ServerUpdate()
        {
            var players = sprites.GetPlayers();

            int playersCount = 0;
            foreach (ServerPlayer player in players)
            {
                if (player.GetService<SpriteHealthController>().IsAlive)
                {
                    playersCount++;
                }
            }

            if (timer.timeToEnd == -1)
            {
                if (playersCount == 0)
                {
                    timer.timeToEnd = 60 * 3;
                }
                else if (players.Count != 1)
                {
                    List<int> live = new List<int>();
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].GetService<SpriteHealthController>().IsAlive)
                        {
                            live.Add(players[i].Team);
                        }
                    }

                    if (Array.TrueForAll(live.ToArray(), val => live[0] == val))
                    {
                        timer.timeToEnd = 60 * 3;
                    }
                }
            }

            if (timer.timeToEnd == 0)
            {
                if (playersCount >= 1)
                {
                    for (int i = 0; i < players.Count; i++)
                    {
                        if (players[i].GetService<SpriteHealthController>().IsAlive)
                        {
                            Winner = players[i].Team;
                        }
                    }
                    Result = GameResult.Victory;
                }
                else
                {
                    Result = GameResult.Draw;
                }
            }

            if (timer.TimeLeft + timer.ApocalypseSpeed * final.MaxApocalypse <= 0)
            {
                Result = GameResult.Draw;
            }
        }
    }

    public class TerrainTimer : IServerGameEntity
    {
        public const int FLAME_ANIMATION_DELAY = 6;

        public int TimeLeft { get; set; }

        public int FlameDuration => 4 * FLAME_ANIMATION_DELAY;
        public int ApocalypseSpeed { get; } = 2;

        public int timeToEnd = -1;

        public TerrainTimer(Map mapData)
        {
            TimeLeft = (mapData.Time + 31) * 60;
        }

        public void ServerUpdate()
        {
            TimeLeft--;

            if (timeToEnd != -1)
            {
                timeToEnd--;
            }
        }
    }
}
