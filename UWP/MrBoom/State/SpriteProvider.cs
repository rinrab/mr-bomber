// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom.State
{
    public interface ISpriteProvider
    {
        IEnumerable<IPlayerState> EnumerateSprites();
    }

    public class SpriteProvider : ISpriteProvider
    {
        private readonly IPlayerProvider players;
        private readonly List<IPlayerState> monsters;

        public SpriteProvider(IPlayerProvider players)
        {
            this.players = players;

            monsters = new List<IPlayerState>();

            for (int i = 0; i < players.MaxPlayers - players.Count; i++)
            {
                monsters.Add(new OnlineMonsterPlayerState(i));
            }
        }

        public IEnumerable<IPlayerState> EnumerateSprites()
        {
            int count = 0;

            for (int i = 0; i < players.Count && count < players.MaxPlayers; i++, count++)
            {
                yield return players[i];
            }

            for (int i = 0; i < monsters.Count && count < players.MaxPlayers; i++, count++)
            {
                yield return monsters[i];
            }
        }
    }
}
