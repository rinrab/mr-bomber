// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom.Screens
{
    public interface IExtensibilityProvider
    {
        IList<ITeamModeProvider> TeamModes { get; }
    }
}
