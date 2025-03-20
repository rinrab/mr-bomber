// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites
{
    public class SpriteBombController : IBombKicker, IBombOwner, IServerGameEntity, IPowerUpHandler
    {
        private readonly TerrainMap map;
        private readonly SpriteEffectController effectController;
        private readonly SpriteHealthController healthController;

        public int BombsPlaced { get; protected set; }

        public int MaxBoom { get; protected set; }
        public int MaxBombsCount { get; protected set; }

        public int BombsRemaining => MaxBombsCount - BombsPlaced;

        public bool RemoteDetonate { get; set; }

        public bool IsAllowed => effectController.Features.HasFlag(
            Feature.RemoteControl) || healthController.IsDie;

        public SpriteBombController(TerrainMap map,
                                    SpriteEffectController effectController,
                                    SpriteHealthController healthController)
        {
            this.map = map;
            this.effectController = effectController;
            this.healthController = healthController;

            MaxBoom = 1;
            MaxBombsCount = 1;
        }

        public virtual bool PutBomb(int cellX, int cellY)
        {
            Cell cell = map.GetCell(cellX, cellY);

            if (cell.Type == TerrainType.Free && BombsPlaced < MaxBombsCount)
            {
                map.PutBomb(cellX, cellY, MaxBoom,
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
            Cell cell = map.GetCell(x, y);
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

        public PowerUpPickResult PickPowerUp(PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.ExtraFire)
            {
                MaxBoom++;
                return PowerUpPickResult.Pick;
            }
            else if (powerUpType == PowerUpType.ExtraBomb)
            {
                MaxBombsCount++;
                return PowerUpPickResult.Pick;
            }
            else
            {
                return PowerUpPickResult.Skip;
            }
        }
    }
}
