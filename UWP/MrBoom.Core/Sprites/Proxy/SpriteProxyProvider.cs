// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Providers;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Proxy
{
    public class SpriteProxyProvider : ISpriteProxy
    {
        private readonly SpritePosition position;
        private readonly SpriteStartInfo startInfo;
        private readonly SpriteAnimationController animationController;
        private readonly SpriteEffectController effectController;
        private readonly SpriteHealthController healthController;

        public SpriteType Type => startInfo.Type;
        public int SubType => startInfo.SubType;

        public int X => position.X;
        public int Y => position.Y;

        public int AnimateIndex => animationController.AnimateIndex;
        public int FrameIndex => animationController.FrameIndex;

        public Feature Features => effectController.Features;
        public SkullType? Skull => effectController.Skull;
        public bool HasSkull => effectController.HasSkull;

        public bool HasUnplugin => healthController.HasUnplugin;

        public bool IsDie => healthController.IsDie;

        public SpriteProxyProvider(SpritePosition position,
                                   SpriteStartInfo startInfo,
                                   SpriteAnimationController animationController,
                                   SpriteEffectController effectController,
                                   SpriteHealthController healthController)
        {
            this.position = position;
            this.startInfo = startInfo;
            this.animationController = animationController;
            this.effectController = effectController;
            this.healthController = healthController;
        }

        public void ClientUpdate()
        {
        }
    }
}
