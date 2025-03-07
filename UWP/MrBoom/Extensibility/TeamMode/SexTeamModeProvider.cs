// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using MrBoom.State;

namespace MrBoom.Screens
{
    public class SexTeamModeProvider : ITeamModeProvider
    {
        public string Name => "SEX";

        public List<Team> CreateTeams(IPlayerProvider playerProvider)
        {
            List<IPlayerState> players = playerProvider.ToList();

            List<Team> teams = new List<Team>
            {
                new Team { Players = new List<IPlayerState>() },
                new Team { Players = new List<IPlayerState>() }
            };

            for (int i = 0; i < players.Count; i += 2)
            {
                teams[0].Players.Add(players[i]);
                if (i + 1 < players.Count)
                {
                    teams[1].Players.Add(players[i + 1]);
                }
            }

            return teams;
        }
    }
}
