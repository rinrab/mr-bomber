// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using MrBoom.State;

namespace MrBoom.Screens
{
    public class ColourTeamModeProvider : ITeamModeProvider
    {
        public string Name => "COLOR";

        public List<Team> CreateTeams(IPlayerProvider playerProvider)
        {
            List<Team> teams = new List<Team>();

            if (playerProvider.Count == 2)
            {
                foreach (IPlayerState player in playerProvider)
                {
                    teams.Add(new Team { Players = new List<IPlayerState> { player } });
                }
            }
            else
            {
                List<IPlayerState> players = playerProvider.ToList();

                for (int i = 0; i < players.Count; i += 2)
                {
                    var newPlayers = new List<IPlayerState> { players[i] };
                    if (i + 1 < players.Count)
                    {
                        newPlayers.Add(players[i + 1]);
                    }

                    teams.Add(new Team { Players = newPlayers });
                }
            }

            return teams;
        }
    }
}
