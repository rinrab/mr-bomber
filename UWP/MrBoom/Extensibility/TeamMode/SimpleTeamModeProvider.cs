// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.State;

namespace MrBoom.Screens
{
    public class SimpleTeamModeProvider : ITeamModeProvider
    {
        public string Name => "OFF";

        public List<Team> CreateTeams(IPlayerProvider playerProvider)
        {
            var teams = new List<Team>();

            foreach (IPlayerState player in playerProvider)
            {
                teams.Add(new Team
                {
                    Players = new List<IPlayerState> { player },
                });
            }

            return teams;
        }
    }
}
