// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public abstract class Sprite : MovableSprite
    {
        public Directions? Direction { get; protected set; }

        public SoundEffectType SoundsToPlay {  get; private set; }
        public bool IsDie { get; private set; } = false;
        public bool IsAlive { get => !IsDie; }

        private readonly int DefaultSpeed;

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

        public virtual void ServerUpdate()
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
            PlaySound(SoundEffectType.Skull);

            skullTimer = 600;
            Skull = skullType;
        }

        public void Kill()
        {
            IsDie = true;
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

        protected void PlaySound(SoundEffectType sound)
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
