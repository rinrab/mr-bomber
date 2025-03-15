// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;

namespace MrBoom.Core.Sprites
{
    public class SpritePowerUpHandler : IServerGameEntity
    {
        protected readonly SpritePosition position;
        protected readonly SpriteEffectController effectController;
        protected readonly SpriteBombController bombController;
        protected readonly SpriteHealthController healthController;
        protected readonly IRandom random;
        protected readonly ITerrain terrain;

        public SpritePowerUpHandler(SpritePosition position,
                                    SpriteEffectController effectController,
                                    SpriteBombController bombController,
                                    SpriteHealthController healthController,
                                    IRandom random, ITerrain terrain)
        {
            this.position = position;
            this.effectController = effectController;
            this.bombController = bombController;
            this.healthController = healthController;
            this.random = random;
            this.terrain = terrain;
        }

        public void ServerUpdate()
        {
            int cellX = (position.X + 8) / 16;
            int cellY = (position.Y + 8) / 16;
            Cell cell = terrain.GetCell(cellX, cellY);

            if (cell.Type == TerrainType.PowerUp)
            {
                if (PickPowerUp(cell.PowerUpType))
                {
                    terrain.SetCell(cellX, cellY, new Cell(TerrainType.Free));
                    //PlaySound(SoundEffectType.Pick);
                }
                else
                {
                    terrain.BurnCell(cellX, cellY);
                }
            }
        }

        /// <summary>
        /// Handles the player picking up a power-up.
        /// </summary>
        /// <param name="powerUpType"></param>
        /// <returns>true if successfully picked the power up, false if the action cannot be done, for example, when we already had this feature</returns>
        public bool PickPowerUp(PowerUpType powerUpType)
        {
            if (powerUpType == PowerUpType.ExtraFire)
            {
                bombController.UpgradeMaxBoom();
                return true;
            }
            else if (powerUpType == PowerUpType.ExtraBomb)
            {
                bombController.UpgradeMaxBombsCount();
                return true;
            }
            else if (powerUpType == PowerUpType.RemoteControl)
            {
                return effectController.PickFeature(Feature.RemoteControl);
            }
            else if (powerUpType == PowerUpType.RollerSkate)
            {
                return effectController.PickFeature(Feature.RollerSkates);
            }
            else if (powerUpType == PowerUpType.Kick)
            {
                return effectController.PickFeature(Feature.Kick);
            }
            else if (powerUpType == PowerUpType.Life)
            {
                healthController.PickExtraLife();
                return true;
            }
            else if (powerUpType == PowerUpType.Shield)
            {
                healthController.PickUnplugin();
                return true;
            }
            else if (powerUpType == PowerUpType.Banana)
            {
                for (int y = 0; y < terrain.Height; y++)
                {
                    for (int x = 0; x < terrain.Width; x++)
                    {
                        if (terrain.GetCell(x, y).Type == TerrainType.Bomb)
                        {
                            terrain.DitonateBomb(x, y);
                        }
                    }
                }

                return true;
            }
            else if (powerUpType == PowerUpType.Clock)
            {
                if (terrain.TimeLeft > 31 * 60 + terrain.MaxApocalypse * terrain.ApocalypseSpeed)
                {
                    // TODO: terrain.TimeLeft += 60 * 60;
                    // PlaySound(SoundEffectType.Clock);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (powerUpType == PowerUpType.Skull)
            {
                effectController.SetSkull(random.NextEnum<SkullType>());
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
