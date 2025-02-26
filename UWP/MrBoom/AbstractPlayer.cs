// Copyright (c) Timofei Zhakov. All rights reserved.

namespace MrBoom
{
    public abstract class AbstractPlayer : Sprite, IServerPlayer
    {
        public int BombsPlaced;
        public bool RemoteDetonate = false;
        public int MaxBoom;
        public int MaxBombsCount;

        public int BombsRemaining
        {
            get
            {
                return MaxBombsCount - BombsPlaced;
            }
        }

        protected bool rcDitonateButton;
        protected bool dropBombButton;

        public int Team;
        public int TeamMask { get => 1 << Team; }

        public AbstractPlayer(Terrain terrain, int team) : base(terrain, 0, 0, 3)
        {
            Features = terrain.StartFeatures;
            MaxBoom = terrain.StartMaxFire;
            MaxBombsCount = terrain.StartMaxBombsCount;
            Team = team;
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
                    PlaySound(Sound.PoseBomb);
                }
            }

            void pickBonus()
            {
                terrain.SetCell(cellX, cellY, new Cell(TerrainType.Free));
                PlaySound(Sound.Pick);
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
                        PlaySound(Sound.Clock);
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
                PlaySound(Sound.PlayerDie);
            }
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
                PlaySound(Sound.Oioi);
            }
            else
            {
                PlaySound(Sound.PlayerDie);
            }
        }
    }
}
