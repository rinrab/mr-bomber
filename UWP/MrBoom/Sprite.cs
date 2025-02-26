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
        public int frameIndex;

        // features
        public Feature Features { get; protected set; }
        public SkullType? Skull { get; protected set; }

        // map source
        protected readonly ITerrainAccessor TerrainAccessor;

        public MovableSprite(ITerrainAccessor terrain, int x, int y)
        {
            X = x;
            Y = y;
            TerrainAccessor = terrain;
        }

        public abstract void KickBomb(int x, int y, int dx, int dy);

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

    public abstract class Sprite : MovableSprite
    {
        public Directions? Direction { get; protected set; }

        private bool isDie = false;
        public Sound SoundsToPlay {  get; private set; }
        public bool IsDie { get => isDie; }
        public bool IsAlive { get => !isDie; }

        public bool HasUnplugin { get => Unplugin > 0; }
        public bool HasSkull { get => skullTimer > 0; }

        private int skullTimer;
        private readonly int DefaultSpeed;

        public int LifeCount { get; set; }
        public int Unplugin { get; set; }

        protected readonly Terrain terrain;

        public Sprite(Terrain terrain, int x, int y, int speed) : base(terrain, x, y)
        {
            Direction = null;
            this.terrain = terrain;
            DefaultSpeed = speed;
        }

        public override void KickBomb(int x, int y, int dx, int dy)
        {
            Cell cell = terrain.GetCell(x, y);
            cell.DeltaX = dx * 2;
            cell.DeltaY = dy * 2;
        }

        public virtual void Update()
        {
            SoundsToPlay = 0;

            if (IsDie)
            {
                frameIndex += 4;
                AnimateIndex = 4;
                skullTimer = 0;
                Skull = null;
                return;
            }

            int speed = DefaultSpeed;
            if (Features.HasFlag(Feature.RollerSkates))
            {
                speed = 4;
            }
            if (Skull == SkullType.Fast)
            {
                speed = 5;
            }
            if (Skull == SkullType.Slow)
            {
                speed = 1;
            }

            if (skullTimer > 0)
            {
                skullTimer--;
            }
            else
            {
                Skull = null;
            }

            if (Unplugin > 0)
            {
                Unplugin--;
            }

            Cell cell = terrain.GetCell((X + 8) / 16, (Y + 8) / 16);

            if (cell.Type == TerrainType.Bomb && cell.OffsetX == 0 && cell.OffsetY == 0)
            {
                cell.DeltaX = 0;
                cell.DeltaY = 0;
            }

            Move(Direction, speed);
        }

        public void SetSkull(SkullType skullType)
        {
            PlaySound(Sound.Skull);

            skullTimer = 600;
            Skull = skullType;
        }

        public void Kill()
        {
            isDie = true;
            Direction = null;
            frameIndex = 0;
            Unplugin = 0;
        }

        public virtual void Damage()
        {
            if (LifeCount > 0)
            {
                LifeCount--;
                Unplugin = 165;
            }
            else if (IsAlive)
            {
                Kill();
            }
        }

        protected void PlaySound(Sound sound)
        {
            SoundsToPlay |= sound;
        }

        public virtual string GetCellDebugInfo(int cellX, int cellY)
        {
            return string.Empty;
        }

        public virtual string GetDebugInfo()
        {
            return string.Empty;
        }
    }
}
