using System;
using Microsoft.Xna.Framework;

namespace Game.Data
{
    public sealed class World
    {
        public int Width => Size.X;
        public int Height => Size.Y;

        public Point Size { get; private set; }
        public float Gravity { get; private set; }
        public int BlockUpdatesPerTick { get; private set; }

        private Block[,] _blockGrid;

        public World(Block[,] blockGrid, float gravity, int blockUpdatesPerTick)
        {
            _blockGrid = blockGrid;
            Size = new Point(_blockGrid.GetLength(1), _blockGrid.GetLength(0));
            Gravity = gravity;
            BlockUpdatesPerTick = blockUpdatesPerTick;
        }

        public ref Block Block(Point position) => ref _blockGrid[position.Y, position.X];

        public (Block block, int y) GetTopBlock(int x)
        {
            for (int i = Height - 1; i >= 0; i--)
            {
                var _block = Block(new Point(x, i));
                if (!_block.CanWalkThrough)
                    return (_block, i);
            }
            return (Blocks.Air, 0);
        }

        public void Update()
        {
            for (int i = 0; i < BlockUpdatesPerTick; i++)
            {
                // get random point
                var pos = Util.Random.NextPoint(Size);
                // update block at that point
                Block(pos).Update(pos, this);
            }
        }

        public void Draw(Display display, Entity player)
        {
            var drawScale = display.ShowGrid ? new Vector2(display.BlockScale - 1) : new Vector2(display.BlockScale);
            // find edge to start drawing
            var visualWidth = (int)MathF.Ceiling(display.WindowSize.X / display.BlockScale) + 4;
            var visualHeight = (int)MathF.Ceiling(display.WindowSize.Y / display.BlockScale) + 4;
            var visualStartX = (int)MathF.Floor(player.Position.X - (visualWidth / 2f));
            var visualStartY = (int)MathF.Ceiling(player.Position.Y - (visualHeight / 2f)) + 2;
            // fix variables if outside of bounds
            if (visualStartX < 0)
            {
                visualWidth += visualStartX;
                visualStartX = 0;
            }
            if (visualStartY < 0)
            {
                visualHeight += visualStartY;
                visualStartY = 0;
            }
            if (visualWidth >= Width - visualStartX)
                visualWidth = Width - visualStartX - 1;
            if (visualHeight >= Height - visualStartY)
                visualHeight = Height - visualStartY - 1;
            // draw each visible block
            for (int y = 0; y < visualHeight; y++)
            {
                for (int x = 0; x < visualWidth; x++)
                {
                    var _x = x + visualStartX;
                    var _y = y + visualStartY;
                    var drawPos = new Vector2(_x * display.BlockScale, (-1 - _y) * display.BlockScale) - display.CameraOffset;
                    var blockPos = new Point(_x, _y);
                    Color color;
                    if (Debug.Enabled && Debug.TrackUpdated && Debug.UpdatedPoints.Contains(blockPos))
                        color = Colors.DebugUpdate;
                    else
                        color = Block(blockPos).Color;
                    display.Draw(drawPos, drawScale, color);
                }
            }
        }
    }
}
