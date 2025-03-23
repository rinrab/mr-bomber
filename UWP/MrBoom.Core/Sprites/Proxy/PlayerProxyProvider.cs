// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Providers;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Proxy
{
    public class PlayerProxyProvider : IPlayerProxy, IServerGameEntity
    {
        private readonly SpritePosition position;
        private readonly SpriteStartInfo startInfo;
        private readonly SpriteAnimationController animationController;
        private readonly SpriteEffectController effectController;
        private readonly SpriteHealthController healthController;
        private readonly SpriteBombController bombController;

        public SpriteType Type => startInfo.Type;
        public int SubType => startInfo.SubType;

        public int X => position.X;
        public int Y => position.Y;

        public int AnimateIndex => animationController.AnimateIndex;
        public int FrameIndex => animationController.FrameIndex;

        public Feature Features => effectController.Features;
        public SkullType? Skull => effectController.Skull;
        public bool HasSkull => effectController.HasSkull;

        public int LifeCount => healthController.LifeCount;
        public bool HasUnplugin => healthController.HasUnplugin;

        public PlayerProxyProvider(SpritePosition position,
                                   SpriteStartInfo startInfo,
                                   SpriteAnimationController animationController,
                                   SpriteEffectController effectController,
                                   SpriteHealthController healthController,
                                   SpriteBombController bombController)
        {
            this.position = position;
            this.startInfo = startInfo;
            this.animationController = animationController;
            this.effectController = effectController;
            this.healthController = healthController;
            this.bombController = bombController;
        }

        public void ServerUpdate()
        {
        }

        public void ClientUpdate()
        {
        }

        public void SetDirection(Directions? direction)
        {
        }

        public void ToggleRemoteControl()
        {
            bombController.ToggleRemoteControl();
        }

        public void ToggleDropBomb()
        {
            bombController.DropBomb();
        }

        public void MoveTo(int x, int y)
        {
            position.MoveTo(x, y);
        }
    }
}
