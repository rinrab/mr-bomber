// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class PlayerController : IServerGameEntity
    {
        private readonly SpriteMovementController movementController;
        private readonly SpriteSpeedProvider speedProvider;
        private readonly SpriteBombController bombController;

        public Directions? Direction { get; protected set; }
        protected bool dropBomb;
        protected bool remoteDetonate;

        public PlayerController(SpriteMovementController movementController,
                                SpriteSpeedProvider speedProvider,
                                SpriteBombController bombController)
        {
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
            bombController.DropBomb();
        }

        public void RemoteDetonate()
        {
            bombController.ToggleRemoteControl();
        }

        public void ServerUpdate()
        {
            movementController.Move(Direction, speedProvider.ProvideActualSpeed());
        }
    }
}
