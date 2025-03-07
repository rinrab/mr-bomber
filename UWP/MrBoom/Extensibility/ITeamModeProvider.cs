// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.State;

namespace MrBoom.Screens
{
    public interface ITeamModeProvider
    {
        string Name { get; }

        List<Team> CreateTeams(IPlayerProvider playerProvider);
    }
}
