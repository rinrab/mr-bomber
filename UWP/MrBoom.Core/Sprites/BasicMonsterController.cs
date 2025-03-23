// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.BehaviorTree;
using MrBoom.Common;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites
{
    public class BasicMonsterController : IServerGameEntity
    {
        private readonly BtSequence tree;

        private readonly ISpritePositionProvider position;
        private readonly SpriteMovementController movementController;
        private readonly TerrainMap terrain;
        private readonly IRandom random;

        public BasicMonsterController(ISpritePositionProvider position,
                                      Map.BasicMonsterData monsterData,
                                      SpriteMovementController movementController,
                                      TerrainMap terrain, IRandom random)
        {
            tree = new BtSequence()
            {
                new DelayNode(monsterData.IsSlowStart ? 120 : 0),
                new BtRepeater(new BtSequence()
                    {
                        new ActionNode(ChooseDirection, nameof(ChooseDirection)),
                        new ActionNode(Walk, nameof(Walk)),
                        new DelayNode(monsterData.WaitAfterTurn, "Think")
                    })
            };

            this.position = position;
            this.movementController = movementController;
            this.terrain = terrain;
            this.random = random;
        }

        public void ServerUpdate()
        {
            tree.Update();
        }

        private BtStatus ChooseDirection()
        {
            for (int i = 0; ; i++)
            {
                Directions dir = random.NextEnum<Directions>();

                if (IsWalkable(dir.DeltaX(), dir.DeltaY()))
                {
                    movementController.SetDirection(dir);
                    return BtStatus.Success;
                }
                if (i >= 32)
                {
                    movementController.SetDirection(null);
                    return BtStatus.Failure;
                }
            }
        }

        private BtStatus Walk()
        {
            if (position.X % 16 == 0 && position.Y % 16 == 0 && random.Next(16) == 0)
            {
                movementController.SetDirection(null);
                return BtStatus.Success;
            }
            else if (!IsWalkable(movementController.Direction.DeltaX(), movementController.Direction.DeltaY()))
            {
                movementController.SetDirection(null);
                return BtStatus.Success;
            }
            else
            {
                return BtStatus.Running;
            }
        }

        bool IsWalkable(int dx, int dy)
        {
            switch (terrain.GetCell((position.X + dx * 8 + 8 + dx) / 16,
                                    (position.Y + dy * 8 + 8 + dy) / 16).Type)
            {
                case TerrainType.Free:
                case TerrainType.PowerUpFire:
                case TerrainType.PowerUp:
                    return true;

                case TerrainType.PermanentWall:
                case TerrainType.PermanentWallTextured:
                case TerrainType.TemporaryWall:
                case TerrainType.Bomb:
                case TerrainType.Fire:
                case TerrainType.Apocalypse:
                case TerrainType.Rubber:
                    return false;

                default: return true;
            }
        }
    }
}
