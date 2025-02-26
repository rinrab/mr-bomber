// Copyright (c) Timofei Zhakov. All rights reserved.

using System;

namespace MrBoom
{
    public class Cell
    {
        public readonly TerrainType Type;
        public int Index;
        public int animateDelay;
        public int bombCountdown;
        public int maxBoom;
        public bool rcAllowed;
        public AbstractPlayer owner;
        public Cell Next;
        public PowerUpType PowerUpType;
        public FlameDirection FlameDirection;
        public int OffsetX;
        public int OffsetY;
        public int DeltaX;
        public int DeltaY;

        public Cell(TerrainType type)
        {
            Type = type;
            Index = -1;
        }

        public AnimatedImage GetImages(Assets assets, Assets.Level levelAssets)
        {
            if (Type == TerrainType.TemporaryWall)
            {
                return levelAssets.Walls;
            }
            else if (Type == TerrainType.PermanentWallTextured)
            {
                return levelAssets.PermanentWalls;
            }
            else if (Type == TerrainType.Bomb)
            {
                return assets.Bomb;
            }
            else if (Type == TerrainType.PowerUp)
            {
                return assets.PowerUps[(int)PowerUpType];
            }
            else if (Type == TerrainType.PowerUpFire)
            {
                return assets.Fire;
            }
            else if (Type == TerrainType.Fire)
            {
                return assets.Flames[(int)FlameDirection];
            }
            else
            {
                return null;
            }
        }

        public int GetAnimationLength()
        {
            if (Type == TerrainType.TemporaryWall)
            {
                return 8;
            }
            else if (Type == TerrainType.PermanentWallTextured)
            {
                return 8;
            }
            else if (Type == TerrainType.Bomb)
            {
                return 4;
            }
            else if (Type == TerrainType.PowerUp)
            {
                return 8;
            }
            else if (Type == TerrainType.PowerUpFire)
            {
                return 7;
            }
            else if (Type == TerrainType.Fire)
            {
                return 4;
            }
            else
            {
                return -1;
            }
        }
    }

    public enum PowerUpType
    {
        Banana,
        ExtraBomb,
        ExtraFire,
        Skull,
        Shield,
        Life,
        RemoteControl,
        Kick,
        RollerSkate,
        Clock,
        MultiBomb,
    }


    public enum FlameDirection
    {
        BoomMid,
        BoomHor,
        BoomLeftEnd,
        BoomRightEnd,
        BoomVert,
        BoomTopEnd,
        BoomBottomEnd,
    }

    [Flags]
    public enum Feature
    {
        MultiBomb = 0x01,
        RemoteControl = 0x02,
        Kick = 0x04,
        RollerSkates = 0x08,
    }

    public enum GameResult
    {
        None,
        Victory,
        Draw,
    }
}
