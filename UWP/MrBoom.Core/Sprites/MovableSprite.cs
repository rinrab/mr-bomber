// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public abstract class MovableSprite : ISprite
    {
        // position
        public int X { get; private set; }
        public int Y { get; private set; }

        // current cell
        public int CellX => (X + 8) / 16;
        public int CellY => (Y + 8) / 16;
        public Cell Cell => TerrainAccessor.GetCell(CellX, CellY);

        // animation
        public int AnimateIndex { get; protected set; }
        public int frameIndex { get; protected set; }

        // features
        public Feature Features { get; protected set; }
        public SkullType? Skull { get; protected set; }

        // metadata
        protected int skullTimer;

        public int LifeCount { get; set; }
        public int Unplugin { get; set; }

        public virtual bool HasUnplugin { get => Unplugin > 0; }
        public virtual bool HasSkull { get => skullTimer > 0; }

        // map source
        protected readonly ITerrainAccessor TerrainAccessor;

        public MovableSprite(ITerrainAccessor terrain, int x, int y)
        {
            X = x;
            Y = y;
            TerrainAccessor = terrain;
        }

        public abstract void KickBomb(int x, int y, int dx, int dy);

        public int GetSpeed(int defaultSpeed)
        {
            if (Features.HasFlag(Feature.RollerSkates))
            {
                return 4;
            }
            else if (Skull == SkullType.Fast)
            {
                return 5;
            }
            else if (Skull == SkullType.Slow)
            {
                return 1;
            }
            else
            {
                return defaultSpeed;
            }
        }

        public void Move(Directions? Direction, int speed)
        {
            void moveY(int delta)
            {
                if (X % 16 == 0)
                {
                    int newY = (delta < 0) ? (Y + delta) / 16 : Y / 16 + 1;
                    int cellX = (X + 8) / 16;
                    int cellY = (Y + 8) / 16;

                    if (TerrainAccessor.IsWalkable(cellX, newY))
                    {
                        Y += delta;
                    }

                    if (newY == cellY && Cell.Type == TerrainType.Bomb)
                    {
                        Y += delta;
                    }
                    else
                    {
                        Cell newCell = TerrainAccessor.GetCell(cellX, newY);
                        if (newCell.Type == TerrainType.Bomb)
                        {
                            if (Features.HasFlag(Feature.Kick))
                            {
                                if (newCell.DeltaX == 0)
                                {
                                    KickBomb(cellX, newY, 0, delta);
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
                if (Y % 16 == 0)
                {
                    int newX = (delta < 0) ? (X + delta) / 16 : X / 16 + 1;
                    int cellX = (X + 8) / 16;
                    int cellY = (Y + 8) / 16;

                    if (TerrainAccessor.IsWalkable(newX, cellY))
                    {
                        X += delta;
                    }

                    if (newX == cellX && Cell.Type == TerrainType.Bomb)
                    {
                        X += delta;
                    }
                    else
                    {
                        Cell newCell = TerrainAccessor.GetCell(newX, cellY);
                        if (newCell.Type == TerrainType.Bomb)
                        {
                            if (Features.HasFlag(Feature.Kick))
                            {
                                if (newCell.DeltaY == 0)
                                {
                                    KickBomb(newX, cellY, delta, 0);
                                    newCell.DeltaX = delta * 2;
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
                frameIndex++;

                int move = 0;
                if (speed == 1)
                    move = (frameIndex % 3 == 0) ? 1 : 0;
                else if (speed == 2)
                    move = (frameIndex % 2 == 0) ? 1 : 0;
                else if (speed == 3)
                    move = 1;
                else if (speed == 4)
                    move = 2;
                else if (speed == 5)
                    move = 4;

                for (int i = 0; i < move; i++)
                {
                    if (Direction == Directions.Up)
                    {
                        AnimateIndex = 3;
                        moveY(-1);
                    }
                    else if (Direction == Directions.Down)
                    {
                        AnimateIndex = 0;
                        moveY(1);
                    }
                    else if (Direction == Directions.Left)
                    {
                        moveX(-1);
                        AnimateIndex = 2;
                    }
                    else if (Direction == Directions.Right)
                    {
                        moveX(1);
                        AnimateIndex = 1;
                    }
                }
            }
            else
            {
                frameIndex = 0;
            }
        }

        void XAlign(int deltaY)
        {
            if (TerrainAccessor.IsWalkable((X - 1) / 16, (Y + 8) / 16 + deltaY))
            {
                X -= 1;
                AnimateIndex = 2;
            }
            else if (TerrainAccessor.IsWalkable((X + 16) / 16, (Y + 8) / 16 + deltaY))
            {
                X += 1;
                AnimateIndex = 1;
            }
        }

        void YAlign(int deltaX)
        {
            if (TerrainAccessor.IsWalkable((X + 8) / 16 + deltaX, (Y - 1) / 16))
            {
                Y -= 1;
                AnimateIndex = 3;
            }
            else if (TerrainAccessor.IsWalkable((X + 8) / 16 + deltaX, (Y + 16) / 16))
            {
                Y += 1;
                AnimateIndex = 0;
            }
        }

        public void MoveTo(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
