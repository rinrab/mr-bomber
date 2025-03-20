// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites.Interface
{
    public interface IPowerUpHandler
    {
        PowerUpPickResult PickPowerUp(PowerUpType powerUpType);
    }
}
