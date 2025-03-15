// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class MonsterController : IServerGameEntity
    {
        private readonly SpriteMovementController movementController;
        private readonly SpriteSpeedProvider speedProvider;

        public Directions? Direction { get; protected set; }

        public MonsterController(SpriteMovementController movementController,
                                 SpriteSpeedProvider speedProvider)
        {
            this.movementController = movementController;
            this.speedProvider = speedProvider;
        }

        public void SetDirection(Directions? direction)
        {
            Direction = direction;
        }

        public void ServerUpdate()
        {
            movementController.Move(Direction, speedProvider.ProvideActualSpeed());
        }
    }
}
