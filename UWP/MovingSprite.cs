﻿using Microsoft.Xna.Framework.Graphics;

namespace MrBoom
{
    public class MovingSprite
    {
        public int x;
        public int y;
        public int speed = 1;
        public bool isHaveKick;
        public Terrain terrain;
        public Directions Direction;
        public int frameIndex;
        public int animateIndex;

        public MovingSprite(Terrain map)
        {
            this.terrain = map;
        }

        public int AnimateIndex { get; private set; }

        public virtual void Update()
        {
            Update(true);
        }

        public virtual void Update(bool move)
        {
            {
                Cell cell = terrain.GetCell((x + 8) / 16, (y + 8) / 16);

                if (cell.Type == TerrainType.Bomb && cell.OffsetX == 0 && cell.OffsetY == 0)
                {
                    cell.DeltaX = 0;
                    cell.DeltaY = 0;
                }
            }

            void moveY(int delta)
            {
                if (this.x % 16 == 0)
                {
                    var newY = (delta < 0) ? (this.y + delta) / 16 : this.y / 16 + 1;
                    var cellX = (this.x + 8) / 16;
                    var cellY = (this.y + 8) / 16;
                    var cell = terrain.GetCell(cellX, cellY);

                    if (terrain.isWalkable(cellX, newY))
                    {
                        this.y += delta;
                    }

                    if (newY == cellY && cell.Type == TerrainType.Bomb)
                    {
                        this.y += delta;
                    }
                    else
                    {
                        Cell newCell = terrain.GetCell(cellX, newY);
                        if (newCell.Type == TerrainType.Bomb)
                        {
                            if (this.isHaveKick)
                            {
                                if (newCell.DeltaX == 0)
                                {
                                    newCell.DeltaY = delta * 2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.XAlign(delta);
                }

                this.frameIndex += 1;
            }
            void moveX(int delta)
            {
                if (this.y % 16 == 0)
                {
                    var newX = (delta < 0) ? (this.x + delta) / 16 : this.x / 16 + 1;
                    var cellX = (this.x + 8) / 16;
                    var cellY = (this.y + 8) / 16;
                    var cell = terrain.GetCell(cellX, cellY);

                    if (terrain.isWalkable(newX, cellY))
                    {
                        this.x += delta;
                    }

                    if (newX == cellX && cell.Type == TerrainType.Bomb)
                    {
                        this.x += delta;
                    }
                    else
                    {
                        Cell newCell = terrain.GetCell(newX, cellY);
                        if (newCell.Type == TerrainType.Bomb)
                        {
                            if (this.isHaveKick)
                            {
                                if (newCell.DeltaY == 0)
                                {
                                    newCell.DeltaX = delta * 2;
                                }
                            }
                        }
                    }
                }
                else
                {
                    this.YAlign(delta);
                }

                this.frameIndex += 1;
            }

            if (move)
            {
                for (int i = 0; i < this.speed; i++)
                {
                    if (this.Direction == Directions.Up)
                    {
                        this.animateIndex = 3;
                        moveY(-1);
                    }
                    else if (this.Direction == Directions.Down)
                    {
                        this.animateIndex = 0;
                        moveY(1);
                    }
                    else if (this.Direction == Directions.Left)
                    {
                        moveX(-1);
                        this.animateIndex = 2;
                    }
                    else if (this.Direction == Directions.Right)
                    {
                        moveX(1);
                        this.animateIndex = 1;
                    }
                    else
                    {
                        this.frameIndex = 0;
                    }
                }
            }
            else
            {
                if (this.Direction == Directions.Up)
                {
                    this.animateIndex = 3;
                }
                else if (this.Direction == Directions.Down)
                {
                    this.animateIndex = 0;
                }
                else if (this.Direction == Directions.Left)
                {
                    this.animateIndex = 2;
                }
                else if (this.Direction == Directions.Right)
                {
                    this.animateIndex = 1;
                }
                else
                {
                    this.frameIndex = 0;
                }
                if (Direction != Directions.None)
                {
                    frameIndex++;
                }
            }
        }

        void XAlign(int deltaY)
        {
            if (terrain.isWalkable((this.x - 1) / 16, (this.y + 8) / 16 + deltaY))
            {
                this.x -= 1;

                this.AnimateIndex = 2;
            }
            else if (terrain.isWalkable((this.x + 16) / 16, (this.y + 8) / 16 + deltaY))
            {
                this.x += 1;

                this.AnimateIndex = 1;
            }
        }

        void YAlign(int deltaX)
        {
            if (terrain.isWalkable((this.x + 8) / 16 + deltaX, (this.y - 1) / 16))
            {
                this.y -= 1;
                this.AnimateIndex = 3;
            }
            else if (terrain.isWalkable((this.x + 8) / 16 + deltaX, (this.y + 16) / 16))
            {
                this.y += 1;

                this.AnimateIndex = 0;
            }
        }

        public enum Directions
        {
            None,
            Up,
            Down,
            Left,
            Right,
        }

        public virtual void Draw(SpriteBatch ctx)
        {
            throw new System.NotImplementedException();
        }
    }
}
