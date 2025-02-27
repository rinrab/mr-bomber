// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public interface ISprite
    {
        int X { get; }
        int Y { get; }

        int AnimateIndex { get; }
        int FrameIndex { get; }

        Feature Features { get; }
        SkullType? Skull { get; }

        int LifeCount { get; }

        bool HasUnplugin { get; }
        bool HasSkull { get; }
    }

    public interface IServerPlayer : ISprite, IServerGameEntity
    {
        void SetDirection(Directions? direction);
        void ToggleRemoteControl();
        void ToggleDropBomb();
        void MoveTo(int x, int y);
    }
}
