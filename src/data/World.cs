using System;
using Microsoft.Xna.Framework;

namespace Game.Data
{
    public sealed class World
    {
        public const float WORLD_UPDATED_PER_SECOND = 1f / 32f;
        public const int TICKS_PER_SECOND = 32;
        public const float GRAVITY = 10f;

        public static readonly float TickStep = 1f / TICKS_PER_SECOND;
        public static Point DefaultSize => new Point(1024, 512);

        public int Width => Size.X;
        public int Height => Size.Y;

        public Point Size { get; private set; }
        public int BlockUpdatesPerTick { get; private set; }

        private Block[,] _blockGrid;

        public World(Block[,] blockGrid)
        {
            _blockGrid = blockGrid;
            Size = new Point(_blockGrid.GetLength(1), _blockGrid.GetLength(0));
            BlockUpdatesPerTick = (int)(((Width * Height) * WORLD_UPDATED_PER_SECOND) / World.TICKS_PER_SECOND);
        }

        public ref Block Block(Point position) => ref _blockGrid[position.Y, position.X];

        public (Block block, int y) GetTopBlock(int x)
        {
            for (int y = Height - 1; y >= 0; y--)
            {
                var _block = Block(new Point(x, y));
                if (!_block.CanWalkThrough)
                    return (_block, y);
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

        public void Draw(Entity player)
        {
            var drawScale = Display.ShowGrid ? new Vector2(Display.BlockScale - 1) : new Vector2(Display.BlockScale);
            // find edge to start drawing
            var visualWidth = (int)MathF.Ceiling(Display.WindowSize.X / Display.BlockScale) + 4;
            var visualHeight = (int)MathF.Ceiling(Display.WindowSize.Y / Display.BlockScale) + 4;
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
                var blockY = y + visualStartY;
                var drawY = (-1 - blockY) * Display.BlockScale;
                for (int x = 0; x < visualWidth; x++)
                {
                    var blockX = x + visualStartX;
                    var drawPos = new Vector2(blockX * Display.BlockScale, drawY) - Display.CameraOffset;
                    var blockPos = new Point(blockX, blockY);
                    Display.Draw(drawPos, drawScale, Debug.Enabled && Debug.TrackUpdated && Debug.UpdatedPoints.Remove(blockPos) ? Colors.Debug_BlockUpdate : Block(blockPos).Color);
                }
            }
        }
    }
}
