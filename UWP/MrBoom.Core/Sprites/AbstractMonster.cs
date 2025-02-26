// Copyright (c) Timofei Zhakov. All rights reserved.

using MrBoom.BehaviorTree;

namespace MrBoom
{
    public abstract class AbstractMonster : Sprite
    {
        protected BtNode tree;

        public int Type { get; }

        protected Terrain terrain;

        public AbstractMonster(Terrain terrain, Map.MonsterData monsterData,
                               int x, int y) : base(terrain, x, y, monsterData.Speed)
        {
            LifeCount = monsterData.LivesCount - 1;
            Type = monsterData.Type;
            this.terrain = terrain;

            if (monsterData.IsSlowStart)
            {
                Unplugin = 120;
            }
        }

        public override void ServerUpdate()
        {
            tree.Update();

            base.ServerUpdate();

            if (IsAlive)
            {
                Cell cell = terrain.GetCell((X + 8) / 16, (Y + 8) / 16);
                if (cell.Type == TerrainType.Fire && Unplugin == 0)
                {
                    Damage();

                    if (IsDie)
                    {
                        terrain.SetCell((X + 8) / 16, (Y + 8) / 16, terrain.GeneratePowerUp(PowerUpType.Life));
                    }
                }
                if (cell.Type == TerrainType.Apocalypse)
                {
                    Kill();
                    PlaySound(SoundEffectType.Ai);
                }
            }
        }

        public override void KickBomb(int x, int y, int dx, int dy)
        {
            Cell cell = terrain.GetCell(x, y);
            cell.DeltaX = dx * 2;
            cell.DeltaY = dy * 2;
        }

        public override string GetDebugInfo()
        {
            return tree.ToString();
        }

        public override void Damage()
        {
            PlaySound(SoundEffectType.Ai);
            base.Damage();
        }
    }
}
