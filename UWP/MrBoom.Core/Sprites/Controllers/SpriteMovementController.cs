// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites
{
    public class SpriteMovementController
    {
        private readonly SpriteAnimationController animationController;
        private readonly SpritePosition position;
        private readonly ITerrainProxy terrain;
        private readonly IEffectProvider effectController;
        private readonly ISpeedProvider speedProvider;
        private readonly IBombKicker bombKicker;
        private readonly IHealthProvider health;

        public Directions? Direction { get; protected set; }

        public SpriteMovementController(SpriteAnimationController animationController,
                                        SpritePosition position,
                                        ITerrainProxy terrain,
                                        IEffectProvider effectController,
                                        ISpeedProvider speedProvider,
                                        IBombKicker bombKicker,
                                        IHealthProvider health)
        {
            this.animationController = animationController;
            this.position = position;
            this.terrain = terrain;
            this.effectController = effectController;
            this.speedProvider = speedProvider;
            this.bombKicker = bombKicker;
            this.health = health;
        }

        public void SetDirection(Directions? direction)
        {
            Direction = direction;
        }

        public void ServerUpdate()
        {
            if (health.IsAlive)
            {
                Move(Direction);
            }
        }

        public void Move(Directions? Direction)
        {
            Move(Direction, speedProvider.ProvideActualSpeed());
        }

        public void Move(Directions? Direction, int speed)
        {
            void moveY(int delta)
            {
                if (position.X % 16 == 0)
                {
                    int newY = (delta < 0) ? (position.Y + delta) / 16 : position.Y / 16 + 1;
                    int cellX = (position.X + 8) / 16;
                    int cellY = (position.Y + 8) / 16;

                    if (terrain.IsWalkable(cellX, newY))
                    {
                        position.Y += delta;
                    }

                    if (newY == cellY && position.Cell.Type == TerrainType.Bomb)
                    {
                        position.Y += delta;
                    }
                    else
                    {
                        Cell newCell = terrain.GetCell(cellX, newY);
                        if (newCell.Type == TerrainType.Bomb)
                        {
                            if (effectController.Features.HasFlag(Feature.Kick))
                            {
                                if (newCell.DeltaX == 0)
                                {
                                    bombKicker.KickBomb(cellX, newY, 0, delta);
                                }
                            }
                        }
                    }
                }
                else
                {
                    XAlign(delta);
                }
            }
            void moveX(int delta)
            {
                if (position.Y % 16 == 0)
                {
                    int newX = (delta < 0) ? (position.X + delta) / 16 : position.X / 16 + 1;
                    int cellX = (position.X + 8) / 16;
                    int cellY = (position.Y + 8) / 16;

                    if (terrain.IsWalkable(newX, cellY))
                    {
                        position.X += delta;
                    }

                    if (newX == cellX && position.Cell.Type == TerrainType.Bomb)
                    {
                        position.X += delta;
                    }
                    else
                    {
                        Cell newCell = terrain.GetCell(newX, cellY);
                        if (newCell.Type == TerrainType.Bomb)
                        {
                            if (effectController.Features.HasFlag(Feature.Kick))
                            {
                                if (newCell.DeltaY == 0)
                                {
                                    bombKicker.KickBomb(newX, cellY, delta, 0);
                                }
                            }
                        }
                    }
                }
                else
                {
                    YAlign(delta);
                }
            }

            if (Direction.HasValue)
            {
                animationController.Animate();

                int move = speedProvider.ProvideMovesCount(speed);

                for (int i = 0; i < move; i++)
                {
                    if (Direction == Directions.Up)
                    {
                        animationController.SetAnimation(3);
                        moveY(-1);
                    }
                    else if (Direction == Directions.Down)
                    {
                        animationController.SetAnimation(0);
                        moveY(1);
                    }
                    else if (Direction == Directions.Left)
                    {
                        moveX(-1);
                        animationController.SetAnimation(2);
                    }
                    else if (Direction == Directions.Right)
                    {
                        moveX(1);
                        animationController.SetAnimation(1);
                    }
                }
            }
            else
            {
                animationController.StopAnimation();
            }
        }

        void XAlign(int deltaY)
        {
            if (terrain.IsWalkable((position.X - 1) / 16, (position.Y + 8) / 16 + deltaY))
            {
                position.X -= 1;
                animationController.SetAnimation(2);
            }
            else if (terrain.IsWalkable((position.X + 16) / 16, (position.Y + 8) / 16 + deltaY))
            {
                position.X += 1;
                animationController.SetAnimation(1);
            }
        }

        void YAlign(int deltaX)
        {
            if (terrain.IsWalkable((position.X + 8) / 16 + deltaX, (position.Y - 1) / 16))
            {
                position.Y -= 1;
                animationController.SetAnimation(3);
            }
            else if (terrain.IsWalkable((position.X + 8) / 16 + deltaX, (position.Y + 16) / 16))
            {
                position.Y += 1;
                animationController.SetAnimation(0);
            }
        }
    }
}
