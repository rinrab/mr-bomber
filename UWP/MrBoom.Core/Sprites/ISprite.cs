﻿// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public interface ISprite
    {
        int X { get; }
        int Y { get; }
    }

    public interface IServerPlayer : ISprite, IServerGameEntity
    {
        void SetDirection(Directions? direction);
        void ToggleRemoteControl();
        void ToggleDropBomb();
    }
}
