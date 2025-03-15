// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprite;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites
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

        protected bool dropBomb;
        protected bool remoteDetonate;

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
            if (dropBomb)
            {
                bombController.PutBomb(position.CellX, position.CellY);
                dropBomb = false;
            }

            if (remoteDetonate)
            {
                bombController.RemoteDetonate = true;
                remoteDetonate = false;
            }
        }

        public void ClientUpdate()
        {
        }

        public void SetDirection(Directions? direction)
        {
        }

        public void ToggleRemoteControl()
        {
            dropBomb = true;
        }

        public void ToggleDropBomb()
        {
            dropBomb = true;
        }

        public void MoveTo(int x, int y)
        {
            position.MoveTo(x, y);
        }
    }
}
