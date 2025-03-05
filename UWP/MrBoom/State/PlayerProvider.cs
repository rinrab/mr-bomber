// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;

namespace MrBoom.State
{
    public class PlayerProvider
    {
        public int MaxPlayers { get; } = 8;

        private readonly List<IPlayerState> players;
        private readonly List<IPlayerState> monsters;

        public int Count => players.Count;
        public IPlayerState this[int index] => players[index];

        public PlayerProvider()
        {
            players = new List<IPlayerState>();
            monsters = new List<IPlayerState>();

            InitializeMonsters();
        }

        public PlayerProvider(List<IPlayerState> players)
        {
            this.players = players;
            monsters = new List<IPlayerState>();

            InitializeMonsters();
        }

        private void InitializeMonsters()
        {
            for (int i = 0; i < MaxPlayers - players.Count; i++)
            {
                monsters.Add(new OnlineMonsterPlayerState(i));
            }
        }

        public bool AddPlayer(Func<int, IPlayerState> providePlayer)
        {
            if (players.Count < MaxPlayers)
            {
                players.Add(providePlayer(players.Count));
                return true;
            }
            else
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].IsReplaceble)
                    {
                        players[i] = providePlayer(i);

                        return true;
                    }
                }

                return false;
            }
        }

        public void Clear()
        {
            players.Clear();
        }

        public IEnumerable<IPlayerState> EnumeratePlayers()
        {
            int count = 0;

            for (int i = 0; i < players.Count && count < MaxPlayers; i++, count++)
            {
                yield return players[i];
            }
        }

        public IEnumerable<IPlayerState> EnumerateSprites()
        {
            int count = 0;

            for (int i = 0; i < players.Count && count < MaxPlayers; i++, count++)
            {
                yield return players[i];
            }

            for (int i = 0; i < monsters.Count && count < MaxPlayers; i++, count++)
            {
                yield return monsters[i];
            }
        }
    }
}
