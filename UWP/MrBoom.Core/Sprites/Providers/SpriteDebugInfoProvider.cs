// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Interface;

namespace MrBoom.Core.Sprites.Providers
{
    public class SpriteDebugInfoProvider : IDebugInfoProvider
    {
        private readonly ISpritePositionProvider position;
        private readonly SpriteHealthController health;

        public SpriteDebugInfoProvider(ISpritePositionProvider position, SpriteHealthController health)
        {
            this.position = position;
            this.health = health;
        }

        public string GetDebugInfo()
        {
            string result = $"({position.X,3},{position.Y,3})/({(position.X + 8) / 16,2},{(position.Y + 8) / 16,2})";

            if (health.IsDie)
            {
                result = "DEAD";
            }

            return $"H:{result}";
        }
    }
}
