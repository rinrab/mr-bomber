// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class PlayerController
    {
        private readonly SpriteMovementController movementController;
        private readonly SpriteBombController bombController;

        public PlayerController(SpriteMovementController movementController,
                                SpriteBombController bombController)
        {
            this.movementController = movementController;
            this.bombController = bombController;
        }

        public void SetDirection(Directions? direction)
        {
            movementController.SetDirection(direction);
        }

        public void DropBomb()
        {
            bombController.DropBomb();
        }

        public void RemoteDetonate()
        {
            bombController.ToggleRemoteControl();
        }
    }
}
