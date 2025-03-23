// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Core.Sprites.Interface;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites.Controllers
{
    public class SpriteBombController : IBombKicker, IBombOwner, IServerGameEntity, IPowerUpHandler
    {
        private readonly TerrainMap map;
        private readonly SpriteEffectController effectController;
        private readonly SpriteHealthController healthController;
        private readonly SpritePosition position;
        private readonly ISoundController soundController;

        public int BombsPlaced { get; protected set; }

        public int MaxBoom { get; protected set; }
        public int MaxBombsCount { get; protected set; }

        public int BombsRemaining => MaxBombsCount - BombsPlaced;

        public bool RemoteDetonate { get; protected set; }

        public bool IsAllowed => effectController.Features.HasFlag(
            Feature.RemoteControl) || healthController.IsDie;

        protected bool tryDropBomb;
        protected bool tryDemoteDetonate;

        public SpriteBombController(TerrainMap map,
                                    SpriteEffectController effectController,
                                    SpriteHealthController healthController,
                                    SpritePosition position,
                                    ISoundController soundController)
        {
            this.map = map;
            this.effectController = effectController;
            this.healthController = healthController;
            this.position = position;
            this.soundController = soundController;

            MaxBoom = 1;
            MaxBombsCount = 1;
        }

        private bool PutBomb(int cellX, int cellY)
        {
            Cell cell = map.GetCell(cellX, cellY);

            if (cell.Type == TerrainType.Free && BombsPlaced < MaxBombsCount)
            {
                map.PutBomb(cellX, cellY, MaxBoom,
                                    effectController.Features.HasFlag(Feature.RemoteControl),
                                    this);

                BombsPlaced++;

                soundController.PlaySound(SoundEffectType.PoseBomb);

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
            if (tryDropBomb)
            {
                PutBomb(position.CellX, position.CellY);
                tryDropBomb = false;
            }

            if (RemoteDetonate)
            {
                RemoteDetonate = false;
            }

            if (tryDemoteDetonate)
            {
                RemoteDetonate = true;
                tryDemoteDetonate = false;
            }
        }

        public void DropBomb()
        {
            tryDropBomb = true;
        }

        public bool ToggleRemoteControl()
        {
            if (effectController.Features.HasFlag(Feature.RemoteControl))
            {
                tryDemoteDetonate = true;
                return true;
            }
            else
            {
                return false;
            }
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
