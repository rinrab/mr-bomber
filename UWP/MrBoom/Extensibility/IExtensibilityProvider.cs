// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.Common;

namespace MrBoom.Screens
{
    public interface IExtensibilityProvider
    {
        IList<ITeamModeProvider> TeamModes { get; }

        IRandom Random { get; set; }
        IRandom SoundRandom { get; set; }
        IRandom LevelRandom { get; set; }
        INameGenerator NameGenerator { get; set; }
    }
}
