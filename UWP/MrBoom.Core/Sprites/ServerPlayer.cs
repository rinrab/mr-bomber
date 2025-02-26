﻿// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.Common;

namespace MrBoom
{
    public class ServerPlayer : Sprite, IServerPlayer
    {
        public int BombsPlaced;
        public int MaxBoom;
        public int MaxBombsCount;

        public int BombsRemaining
        {
            get
            {
                return MaxBombsCount - BombsPlaced;
            }
        }

        public bool RemoteDetonate { get; private set; }
        protected bool rcDitonateButton;
        protected bool dropBombButton;

        public int Team;
        public int TeamMask { get => 1 << Team; }

        protected readonly Terrain terrain;

        public ServerPlayer(Terrain terrain, int team) : base(terrain, 0, 0, 3)
        {
            Features = terrain.StartFeatures;
            MaxBoom = terrain.StartMaxFire;
            MaxBombsCount = terrain.StartMaxBombsCount;
            Team = team;
            RemoteDetonate = false;
            this.terrain = terrain;
        }

        public override void ServerUpdate()
        {
            if (IsDie)
            {
                base.ServerUpdate();
                return;
            }

            if (Skull == SkullType.Reverse)
            {
                Direction = Direction.Reverse();
            }

            RemoteDetonate = Features.HasFlag(Feature.RemoteControl) && rcDitonateButton;

            base.ServerUpdate();

            int cellX = (X + 8) / 16;
            int cellY = (Y + 8) / 16;
            Cell cell = terrain.GetCell(cellX, cellY);

            if ((dropBombButton || Skull == SkullType.AutoBomb) && Skull != SkullType.BombsDisable)
            {
                if (cell.Type == TerrainType.Free && BombsPlaced < MaxBombsCount)
                {
                    terrain.PutBomb(cellX, cellY, MaxBoom, Features.HasFlag(Feature.RemoteControl), this);

                    BombsPlaced++;
                    PlaySound(SoundEffectType.PoseBomb);
                }
            }

            void pickBonus()
            {
                terrain.SetCell(cellX, cellY, new Cell(TerrainType.Free));
                PlaySound(SoundEffectType.Pick);
            }

            if (cell.Type == TerrainType.PowerUp)
            {
                PowerUpType powerUpType = cell.PowerUpType;

                if (powerUpType == PowerUpType.ExtraFire)
                {
                    MaxBoom++;
                    pickBonus();
                }
                else if (powerUpType == PowerUpType.ExtraBomb)
                {
                    MaxBombsCount++;
                    pickBonus();
                }
                else if (powerUpType == PowerUpType.RemoteControl)
                {
                    if (!Features.HasFlag(Feature.RemoteControl))
                    {
                        Features |= Feature.RemoteControl;
                        pickBonus();
                    }
                    else
                    {
                        terrain.BurnCell(cellX, cellY);
                    }
                }
                else if (powerUpType == PowerUpType.RollerSkate)
                {
                    if (!Features.HasFlag(Feature.RollerSkates))
                    {
                        Features |= Feature.RollerSkates;
                        pickBonus();
                    }
                    else
                    {
                        terrain.BurnCell(cellX, cellY);
                    }
                }
                else if (powerUpType == PowerUpType.Kick)
                {
                    if (!Features.HasFlag(Feature.Kick))
                    {

                        Features |= Feature.Kick;
                        pickBonus();
                    }
                    else
                    {
                        terrain.BurnCell(cellX, cellY);
                    }
                }
                else if (powerUpType == PowerUpType.Life)
                {
                    LifeCount++;
                    pickBonus();
                }
                else if (powerUpType == PowerUpType.Shield)
                {
                    Unplugin = 600;
                    pickBonus();
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
                    pickBonus();
                }
                else if (powerUpType == PowerUpType.Clock)
                {
                    if (terrain.TimeLeft > 31 * 60 + terrain.MaxApocalypse * terrain.ApocalypseSpeed)
                    {
                        // TODO: terrain.TimeLeft += 60 * 60;
                        PlaySound(SoundEffectType.Clock);
                        pickBonus();
                    }
                    else
                    {
                        terrain.BurnCell(cellX, cellY);
                    }
                }
                else if (powerUpType == PowerUpType.Skull)
                {
                    SetSkull(Terrain.Random.NextEnum<SkullType>());
                    pickBonus();
                }
            }

            bool isTouchingMonster = terrain.IsTouchingMonster((X + 8) / 16, (Y + 8) / 16);

            if (cell.Type == TerrainType.Apocalypse)
            {
                Kill();
                PlaySound(SoundEffectType.PlayerDie);
            }

            dropBombButton = false;
            rcDitonateButton = false;
        }

        public void GiveAll()
        {
            Features |= Feature.RemoteControl | Feature.Kick;
            SetSkull(SkullType.Fast);
        }

        public override void Damage()
        {
            Features = 0;
            base.Damage();

            if (IsAlive)
            {
                PlaySound(SoundEffectType.Oioi);
            }
            else
            {
                PlaySound(SoundEffectType.PlayerDie);
            }
        }

        public void SetDirection(Directions? direction)
        {
            Direction = direction;
        }

        public void ToggleRemoteControl()
        {
            rcDitonateButton = true;
        }

        public void ToggleDropBomb()
        {
            dropBombButton = true;
        }

        public override void KickBomb(int x, int y, int dx, int dy)
        {
            Cell cell = terrain.GetCell(x, y);
            cell.DeltaX = dx * 2;
            cell.DeltaY = dy * 2;
        }
    }
}
