// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites
{
    public class SpriteBombController : IBombKicker, IBombOwner, IServerGameEntity
    {
        private readonly MrBoom.Terrain terrain;
        private readonly SpriteEffectController effectController;
        private readonly SpriteHealthController healthController;

        public int BombsPlaced { get; protected set; }

        public int MaxBoom { get; protected set; }
        public int MaxBombsCount { get; protected set; }

        public int BombsRemaining => MaxBombsCount - BombsPlaced;

        public bool RemoteDetonate { get; set; }

        public bool IsAllowed => effectController.Features.HasFlag(
            Feature.RemoteControl) || healthController.IsDie;

        public SpriteBombController(MrBoom.Terrain terrain,
                                    SpriteEffectController effectController,
                                    SpriteHealthController healthController)
        {
            this.terrain = terrain;
            this.effectController = effectController;
            this.healthController = healthController;

            MaxBoom = 1;
            MaxBombsCount = 1;
        }

        public virtual bool UpgradeMaxBoom()
        {
            MaxBoom++;
            return true;
        }

        public virtual bool UpgradeMaxBombsCount()
        {
            MaxBombsCount++;
            return true;
        }

        public virtual bool PutBomb(int cellX, int cellY)
        {
            Cell cell = terrain.GetCell(cellX, cellY);

            if (cell.Type == TerrainType.Free && BombsPlaced < MaxBombsCount)
            {
                terrain.PutBomb(cellX, cellY, MaxBoom,
                                effectController.Features.HasFlag(Feature.RemoteControl),
                                this);

                BombsPlaced++;

                //PlaySound(SoundEffectType.PoseBomb);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void KickBomb(int x, int y, int dx, int dy)
        {
            Cell cell = terrain.GetCell(x, y);
            cell.DeltaX = dx * 2;
            cell.DeltaY = dy * 2;
        }

        public void OnBombReplaced(int x, int y)
        {
            BombsPlaced--;
        }

        public void ServerUpdate()
        {
            RemoteDetonate = false;
        }
    }
}
