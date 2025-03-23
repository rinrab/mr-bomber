// Copyright (c) Timofei Zhakov. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using MrBoom.BehaviorTree;
using MrBoom.Common;
using MrBoom.Core;
using MrBoom.Core.Sprites;
using MrBoom.Core.Sprites.Controllers;
using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Sprites.Providers;
using MrBoom.Core.Sprites.Proxy;
using MrBoom.Core.Terrain;

namespace MrBoom.Bot
{
    public class ComputerPlayer : ServerPlayer
    {
        public ComputerPlayer(int team, int index, int botSeed) : base(team, index, new ClientInfoFake())
        {
            AddSingleton(new SimpleRandom(botSeed));
            AddSingleton<ComputerPlayerController>();
            AddSingleton<SpriteProxyProvider>();
        }
    }

    public class ComputerPlayerController : IServerGameEntity
    {
        private readonly BtNode tree;
        private readonly TravelCostGrid travelCostGrid;
        private readonly TravelCostGrid travelSafeCostGrid;
        private readonly int botSeed;
        private readonly TravelCostGrid findPathCost;
        private readonly Grid<int> bestExplosionGrid;
        private readonly Grid<bool> dangerGrid;
        private readonly Grid<int> flamesGrid;

        private readonly TerrainMap terrain;
        private readonly TerrainAIInfoProvider aIInfoProvider;
        private readonly TerrainFinal final;
        private readonly IRandom random;
        private readonly IEffectProvider effectProvider;
        private readonly SpritePosition position;
        private readonly SpriteMovementController movementController;
        private readonly SpriteSpeedProvider speedProvider;
        private readonly SpriteBombController bombController;

        public ComputerPlayerController(TerrainMap terrain,
                                        TerrainAIInfoProvider aIInfoProvider,
                                        TerrainFinal final,
                                        IRandom random,
                                        IEffectProvider effectProvider,
                                        SpritePosition position,
                                        SpriteMovementController movementController,
                                        SpriteSpeedProvider speedProvider,
                                        SpriteBombController bombController)
        {
            tree = new BtRepeater(new BtSelector()
                {
                    new ActionNode(GotoBonusCell, nameof(GotoBonusCell)),
                    new BtSequence("Bomb")
                    {
                        new ActionNode(HasBombsLeft, nameof(HasBombsLeft)),
                        new ActionNode(GotoBestBombCell, nameof(GotoBestBombCell)),
                        new ActionNode(DropBomb, nameof(DropBomb), true)
                    },
                    new BtSequence("PostBomb")
                    {
                        new ActionNode(GotoSafeCell, nameof(GotoSafeCell)),
                        new ActionNode(DitonoteRemoteBomb, nameof(DitonoteRemoteBomb))
                    }
                }, "BotMainLoop");

            travelCostGrid = new TravelCostGrid(terrain.Width, terrain.Height);
            travelSafeCostGrid = new TravelCostGrid(terrain.Width, terrain.Height);
            findPathCost = new TravelCostGrid(terrain.Width, terrain.Height);
            bestExplosionGrid = new Grid<int>(terrain.Width, terrain.Height);
            dangerGrid = new Grid<bool>(terrain.Width, terrain.Height, false);
            flamesGrid = new Grid<int>(terrain.Width, terrain.Height, TravelCostGrid.CostCantGo);

            GetDecisionRandom().Shuffle(DirectionsExtensions.All());

            this.terrain = terrain;
            this.aIInfoProvider = aIInfoProvider;
            this.final = final;
            this.random = random;
            this.effectProvider = effectProvider;
            this.position = position;
            this.movementController = movementController;
            this.speedProvider = speedProvider;
            this.bombController = bombController;
        }

        private BtStatus DitonoteRemoteBomb()
        {
            if (bombController.ToggleRemoteControl())
            {
                return BtStatus.Success;
            }
            else
            {
                return BtStatus.Failure;
            }
        }

        private BtStatus GotoBestBombCell()
        {
            return Goto(GetBestBombCell());
        }

        public virtual void ServerUpdate()
        {
            dangerGrid.Reset();
            flamesGrid.Reset();

            void SimulateBomb(int startX, int startY)
            {
                Cell cell = terrain.GetCell(startX, startY);

                bool isBombDanger = cell.owner != this && cell.rcAllowed;

                foreach (Directions dir in DirectionsExtensions.All())
                {
                    for (int k = 0; k <= cell.maxBoom; k++)
                    {
                        int x = startX + dir.DeltaX() * k;
                        int y = startY + dir.DeltaY() * k;

                        dangerGrid[x, y] = true;

                        flamesGrid[x, y] = Math.Min(cell.bombCountdown, flamesGrid[x, y]);
                        if (isBombDanger)
                        {
                            // TODO: don't go here
                        }

                        TerrainType type = terrain.GetCell(x, y).Type;

                        if (type == TerrainType.TemporaryWall ||
                            type == TerrainType.PermanentWall ||
                            type == TerrainType.PermanentWallTextured)
                        {
                            break;
                        }

                        if (type == TerrainType.Bomb)
                        {
                            // TODO:
                        }
                    }
                }
            }

            for (int i = 0; i < dangerGrid.Width; i++)
            {
                for (int j = 0; j < dangerGrid.Height; j++)
                {
                    TerrainType type = terrain.GetCell(i, j).Type;
                    if (type == TerrainType.Bomb)
                    {
                        SimulateBomb(i, j);
                    }
                    else if (type == TerrainType.Fire)
                    {
                        dangerGrid[i, j] = true;
                    }
                    else if (IsCellDangerForApocalypse(i, j))
                    {
                        dangerGrid[i, j] = true;
                    }
                }
            }

            travelCostGrid.Update(position.CellX, position.CellY, CalcTravelCost);
            travelSafeCostGrid.Update(position.CellX, position.CellY, CalcSafeTravelCost);

            bestExplosionGrid.Reset();
            for (int i = 0; i < bestExplosionGrid.Width; i++)
            {
                for (int j = 0; j < bestExplosionGrid.Height; j++)
                {
                    int score = 0;
                    if (!dangerGrid[i, j] && travelCostGrid.CanWalk(i, j))
                    {
                        score++;
                        Grid<bool> flameGrid = new Grid<bool>(bestExplosionGrid.Width, bestExplosionGrid.Height);

                        // Simple naive flame simulation.
                        // TODO: Improve it.
                        foreach (Directions dir in DirectionsExtensions.All())
                        {
                            for (int k = 0; k <= bombController.MaxBoom; k++)
                            {
                                int x = i + dir.DeltaX() * k;
                                int y = j + dir.DeltaY() * k;

                                if (!flameGrid[x, y])
                                {
                                    Cell cell = terrain.GetCell(x, y);

                                    switch (cell.Type)
                                    {
                                        case TerrainType.TemporaryWall:
                                            score += 2;
                                            break;
                                        case TerrainType.Bomb:
                                            score += 1;
                                            break;
                                    }

                                    int killablePlayers = aIInfoProvider.GetKillablePlayers(x, y);
                                    //if ((killablePlayers & (~TeamMask)) != 0)
                                    //{
                                    //    score += 8;
                                    //}

                                    if (aIInfoProvider.IsTouchingMonster(x, y))
                                    {
                                        score += 6;
                                    }

                                    if (cell.Type != TerrainType.Free)
                                    {
                                        break;
                                    }

                                    flameGrid[x, y] = true;
                                }
                            }
                        }

                        // find safe place
                        bool foundSafePlace = false;

                        for (int xx = 0; xx < bestExplosionGrid.Width; xx++)
                        {
                            for (int yy = 0; yy < bestExplosionGrid.Height; yy++)
                            {
                                if (travelCostGrid.CanWalk(xx, yy) && !flameGrid[xx, yy] && !dangerGrid[xx, yy])
                                {
                                    foundSafePlace = true;
                                }
                            }
                        }

                        if (!foundSafePlace)
                        {
                            score = -score;
                        }
                    }

                    bestExplosionGrid[i, j] = score;
                }
            }

            movementController.SetDirection(null);

            tree.Update();

            //if (effectProvider.Skull == SkullType.Reverse)
            //{
            //    Direction = Direction.Reverse();
            //}
        }

        private bool IsCellDangerForApocalypse(int cellX, int cellY)
        {
            return final.GetCellApocalypseRemainingTime(cellX, cellY) < 5 * 60;
        }

        private IRandom GetDecisionRandom()
        {
            return new SimpleRandom(botSeed);
        }

        private int CalcTravelCost(int x, int y)
        {
            if (!terrain.IsWalkable(x, y))
            {
                return TravelCostGrid.CostCantGo;
            }

            if (terrain.GetCell(x, y).Type == TerrainType.Fire)
            {
                return TravelCostGrid.CostCantGo;
            }

            if (aIInfoProvider.IsTouchingMonster(x, y) || aIInfoProvider.IsMonsterComing(x, y))
            {
                return TravelCostGrid.CostCantGo;
            }

            if (flamesGrid[x, y] < 16) // TODO: Correct time
            {
                return TravelCostGrid.CostCantGo;
            }

            return 1;
        }

        private int CalcSafeTravelCost(int x, int y)
        {
            if (dangerGrid[x, y])
                return TravelCostGrid.CostCantGo;

            return CalcTravelCost(x, y);
        }

        private BtStatus GotoSafeCell()
        {
            return Goto(GetSafeCell());
        }

        private Directions? CalcPathDirection(CellCoord target)
        {
            findPathCost.Update(target.X, target.Y,
                (x, y) => (x == position.CellX && y == position.CellY) ? 1 : CalcSafeTravelCost(x, y));

            var result = findPathCost.GetBestDirection(position.CellX, position.CellY, DirectionsExtensions.All());
            if (result == null)
            {
                findPathCost.Update(target.X, target.Y,
                    (x, y) => (x == position.CellX && y == position.CellY) ? 1 : CalcTravelCost(x, y));

                result = findPathCost.GetBestDirection(position.CellX, position.CellY, DirectionsExtensions.All());
            }

            return result;
        }

        private BtStatus Goto(CellCoord? target)
        {
            if (target.HasValue)
            {
                int cellX = (position.X + 8) / 16;
                int cellY = (position.Y + 8) / 16;

                if (target.Value.X == cellX &&
                    target.Value.Y == cellY)
                {
                    const int MAX_PIXELS_PER_FRAME = 8;

                    int targetX = target.Value.X * 16;
                    int targetY = target.Value.Y * 16;

                    if (Math.Abs(targetX - position.X) < MAX_PIXELS_PER_FRAME / 2 && Math.Abs(targetY - position.Y) < MAX_PIXELS_PER_FRAME / 2)
                    {
                        movementController.SetDirection(null);
                        return BtStatus.Success;
                    }

                    if (position.X > targetX)
                    {
                        movementController.SetDirection(Directions.Left);
                        return BtStatus.Running;
                    }
                    else if (position.X < targetX)
                    {
                        movementController.SetDirection(Directions.Right);
                        return BtStatus.Running;
                    }
                    else if (position.Y > targetY)
                    {
                        movementController.SetDirection(Directions.Up);
                        return BtStatus.Running;
                    }
                    else if (position.Y < targetY)
                    {
                        movementController.SetDirection(Directions.Down);
                        return BtStatus.Running;
                    }
                    else
                    {
                        movementController.SetDirection(null);
                        return BtStatus.Success;
                    }
                }
                else
                {
                    Directions? direction = CalcPathDirection(target.Value);
                    movementController.SetDirection(direction);

                    if (direction == null)
                    {
                        return BtStatus.Failure;
                    }
                    else
                    {
                        return BtStatus.Running;
                    }
                }
            }
            else
            {
                // TODO:
                movementController.SetDirection(null);

                return BtStatus.Failure;
            }
        }

        private BtStatus GotoBonusCell()
        {
            return Goto(GetBonusCell());
        }

        private BtStatus DropBomb()
        {
            movementController.SetDirection(null);
            bombController.DropBomb();
            return BtStatus.Success;
        }

        private BtStatus HasBombsLeft()
        {
            if (bombController.BombsRemaining > 0)
            {
                return BtStatus.Success;
            }
            else
            {
                return BtStatus.Failure;
            }
        }

        private CellCoord? GetBestBombCell()
        {
            List<CellCoord> bestCell = new List<CellCoord>();
            int bestScore = 0;

            for (int x = 0; x < bestExplosionGrid.Width; x++)
            {
                for (int y = 0; y < bestExplosionGrid.Height; y++)
                {
                    if (terrain.GetCell(x, y).Type != TerrainType.Bomb)
                    {
                        int score = bestExplosionGrid[x, y] * 128;
                        if (score < 0)
                            score = 0;
                        int travelCost = 1 + travelCostGrid.GetCost(x, y) / 2;
                        if (score > travelCost)
                        {
                            score /= travelCost;
                        }

                        if (score >= bestScore)
                        {
                            if (score > bestScore)
                            {
                                bestCell.Clear();
                            }

                            bestCell.Add(new CellCoord(x, y));
                            bestScore = score;
                        }
                    }
                }
            }

            if (bestCell.Count > 0)
            {
                return GetDecisionRandom().NextElement(bestCell);
            }
            else
            {
                return null;
            }
        }

        private CellCoord? GetSafeCell()
        {
            List<CellCoord> bestCell = new List<CellCoord>();
            int bestScore = int.MinValue;

            for (int x = 0; x < dangerGrid.Width; x++)
            {
                for (int y = 0; y < dangerGrid.Height; y++)
                {
                    if (!dangerGrid[x, y] && travelCostGrid.CanWalk(x, y))
                    {
                        int score = -travelCostGrid.GetCost(x, y);
                        Cell cell = terrain.GetCell(x, y);
                        if (cell.Type == TerrainType.PowerUp && cell.PowerUpType == PowerUpType.Skull)
                        {
                            score *= 2;
                        }

                        if (score >= bestScore && IsCellSafe(x, y))
                        {
                            if (score > bestScore)
                            {
                                bestCell.Clear();
                            }

                            bestCell.Add(new CellCoord(x, y));
                            bestScore = score;
                        }
                    }
                }
            }

            if (bestCell.Count > 0)
            {
                return GetDecisionRandom().NextElement(bestCell);
            }
            else
            {
                return null;
            }
        }

        private bool IsInterestingBonus(PowerUpType bonusType)
        {
            switch (bonusType)
            {
                case PowerUpType.Banana:
                    return true;
                case PowerUpType.ExtraBomb:
                    return true;
                case PowerUpType.ExtraFire:
                    return true;
                case PowerUpType.Skull:
                    return false;
                case PowerUpType.Shield:
                    return true;
                case PowerUpType.Life:
                    return true;
                case PowerUpType.RemoteControl:
                    return !effectProvider.Features.HasFlag(Feature.RemoteControl);
                case PowerUpType.Kick:
                    return !effectProvider.Features.HasFlag(Feature.Kick);
                case PowerUpType.RollerSkate:
                    return !effectProvider.Features.HasFlag(Feature.RollerSkates);
                case PowerUpType.Clock:
                    return false;
                case PowerUpType.MultiBomb:
                    return !effectProvider.Features.HasFlag(Feature.MultiBomb);
                default:
                    return false;
            }
        }

        private int CalcBonusScore(PowerUpType bonusType, int distance)
        {
            if (distance == TravelCostGrid.CostCantGo)
                return 0;

            if (!IsInterestingBonus(bonusType))
                return 0;

            switch (bonusType)
            {
                case PowerUpType.Kick:
                case PowerUpType.RemoteControl:
                case PowerUpType.Shield:
                    distance /= 4;
                    break;

                case PowerUpType.Life:
                case PowerUpType.RollerSkate:
                    distance /= 8;
                    break;
            }

            return TravelCostGrid.CostCantGo - distance;
        }

        private CellCoord? GetBonusCell()
        {
            List<CellCoord> bestCell = new List<CellCoord>();
            int bestScore = 0;

            for (int x = 0; x < terrain.Width; x++)
            {
                for (int y = 0; y < terrain.Height; y++)
                {
                    Cell cell = terrain.GetCell(x, y);
                    if (cell.Type == TerrainType.PowerUp && IsCellSafe(x, y))
                    {
                        int distance = travelSafeCostGrid.GetCost(x, y);
                        int score = CalcBonusScore(cell.PowerUpType, distance);
                        if (score >= bestScore)
                        {
                            if (score > bestScore)
                            {
                                bestCell.Clear();
                            }

                            bestCell.Add(new CellCoord(x, y));
                            bestScore = score;
                        }
                    }
                }
            }

            if (bestCell.Count > 0)
            {
                return GetDecisionRandom().NextElement(bestCell);
            }
            else
            {
                return null;
            }
        }

        private bool IsCellSafe(int x, int y)
        {
            return !dangerGrid[x, y] && !aIInfoProvider.IsTouchingMonster(x, y) && !aIInfoProvider.IsMonsterComing(x, y);
        }

        public string GetCellDebugInfo(int cellX, int cellY)
        {
            StringBuilder sb = new StringBuilder();

            int time = flamesGrid[cellX, cellY];

            if (time != TravelCostGrid.CostCantGo)
            {
                sb.AppendFormat("{0,3}", time);
            }
            else
            {
                sb.Append("   ");
            }

            sb.Append(dangerGrid[cellX, cellY] ? "D" : " ");

            return sb.ToString();
        }

        public string GetDebugInfo()
        {
            //if (GetService<SpriteHealthController>().IsAlive)
            //{
            //    return tree.ToString();
            //}
            //else
            //{
            //    return "DEAD";
            //}
            return null;
        }
    }
}
