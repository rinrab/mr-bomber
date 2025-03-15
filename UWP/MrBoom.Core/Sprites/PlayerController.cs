// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class PlayerController : IServerGameEntity
    {
        private readonly SpritePosition position;
        private readonly SpriteMovementController movementController;
        private readonly SpriteSpeedProvider speedProvider;
        private readonly SpriteBombController bombController;

        public Directions? Direction { get; protected set; }
        protected bool dropBomb;
        protected bool remoteDetonate;

        public PlayerController(SpritePosition position,
                                SpriteMovementController movementController,
                                SpriteSpeedProvider speedProvider,
                                SpriteBombController bombController)
        {
            this.position = position;
            this.movementController = movementController;
            this.speedProvider = speedProvider;
            this.bombController = bombController;
        }

        public void SetDirection(Directions? direction)
        {
            Direction = direction;
        }

        public void DropBomb()
        {
            dropBomb = true;
        }

        public void RemoteDetonate()
        {
            remoteDetonate = true;
        }

        public void ServerUpdate()
        {
            movementController.Move(Direction, speedProvider.ProvideActualSpeed());

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
    }
}
