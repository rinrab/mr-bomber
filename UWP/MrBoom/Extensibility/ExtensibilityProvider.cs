// Copyright (c) Timofei Zhakov. All rights reserved.

using System.Collections.Generic;
using MrBoom.Common;
using MrBoom.Core.Terrain;
using MrBoom.Extensibility.Proxy;

namespace MrBoom.Screens
{
    public class ExtensibilityProvider : IExtensibilityProvider
    {
        public IList<ITeamModeProvider> TeamModes { get; }

        public IRandom Random { get; set; }
        public IRandom SoundRandom { get; set; }
        public IRandom LevelRandom { get; set; }
        public INameGenerator NameGenerator { get; set; }

        public ExtensibilityProvider()
        {
            TeamModes = new List<ITeamModeProvider>();

            Random = new SimpleRandom();
            SoundRandom = new UnrepeatableRandom();
            LevelRandom = new UnrepeatableRandom();
            NameGenerator = new NameGenerator(Random);
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

        public ISpriteProxy WrapSpriteProxy(ISpriteProxy proxy)
        {
            return new SmoothProxy(proxy);
        }

        public IPlayerProxy WrapPlayerProxy(IPlayerProxy proxy)
        {
            return proxy;
        }
    }
}
