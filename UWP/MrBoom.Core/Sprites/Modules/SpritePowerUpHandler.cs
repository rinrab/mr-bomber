// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;
using MrBoom.Core.Terrain;

namespace MrBoom.Core.Sprites
{
    public class SpritePowerUpHandler : IServerGameEntity
    {
        protected readonly SpritePosition position;
        protected readonly SpriteEffectController effectController;
        protected readonly SpriteBombController bombController;
        protected readonly SpriteHealthController healthController;
        protected readonly IRandom random;
        protected readonly TerrainMap map;
        protected readonly TerrainTimer timer;
        protected readonly TerrainFinal final;

        public SpritePowerUpHandler(SpritePosition position,
                                    SpriteEffectController effectController,
                                    SpriteBombController bombController,
                                    SpriteHealthController healthController,
                                    IRandom random,
                                    TerrainMap map,
                                    TerrainTimer timer,
                                    TerrainFinal final)
        {
            this.position = position;
            this.effectController = effectController;
            this.bombController = bombController;
            this.healthController = healthController;
            this.random = random;
            this.map = map;
            this.timer = timer;
            this.final = final;
        }

        public void ServerUpdate()
        {
            int cellX = (position.X + 8) / 16;
            int cellY = (position.Y + 8) / 16;
            Cell cell = map.GetCell(cellX, cellY);

            if (cell.Type == TerrainType.PowerUp)
            {
                if (PickPowerUp(cell.PowerUpType))
                {
                    map.SetCell(cellX, cellY, new Cell(TerrainType.Free));
                    //PlaySound(SoundEffectType.Pick);
                }
                else
                {
                    map.BurnCell(cellX, cellY);
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
                for (int y = 0; y < map.Height; y++)
                {
                    for (int x = 0; x < map.Width; x++)
                    {
                        if (map.GetCell(x, y).Type == TerrainType.Bomb)
                        {
                            map.DitonateBomb(x, y);
                        }
                    }
                }

                return true;
            }
            else if (powerUpType == PowerUpType.Clock)
            {
                if (timer.TimeLeft > 31 * 60 + final.MaxApocalypse * timer.ApocalypseSpeed)
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
