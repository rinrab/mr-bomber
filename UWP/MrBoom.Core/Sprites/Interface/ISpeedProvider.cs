// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom.Core.Sprites.Interface
{
    public interface ISpeedProvider
    {
        int ProvideActualSpeed();
        int ProvideMovesCount(int speed);
        int ProvideMovesCount();
    }
}
