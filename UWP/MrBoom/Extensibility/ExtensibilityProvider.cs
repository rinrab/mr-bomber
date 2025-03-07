// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;

namespace MrBoom.Screens
{
    public class ExtensibilityProvider : IExtensibilityProvider
    {
        public IList<ITeamModeProvider> TeamModes { get; }

        public ExtensibilityProvider()
        {
            TeamModes = new List<ITeamModeProvider>();
        }

        // Default
        public static IExtensibilityProvider Default = InitializeDefaultProvider();

        private static IExtensibilityProvider InitializeDefaultProvider()
        {
            return new ExtensibilityProvider
            {
                TeamModes =
                {
                    new SimpleTeamModeProvider(),
                    new ColourTeamModeProvider(),
                    new SexTeamModeProvider(),
                },
            };
        }
    }
}
