using System;
using Microsoft.Xna.Framework;
using Minicraft.Font;
using Minicraft.Game.Blocks;
using Minicraft.Game.Entities;
using Minicraft.Texture;
using Minicraft.Utils;

namespace Minicraft.Game.Worlds
{
    public sealed partial class World
    {
        public const float WORLD_UPDATED_PER_SECOND = 1f / 32f;
        public const int TICKS_PER_SECOND = 32;
        public const float GRAVITY = 10f;
        public const int WIDTH = 1024;
        public const int HEIGHT = 512;
        public const float TICK_STEP = 1f / TICKS_PER_SECOND;
        private const int BLOCK_UPDATES_PER_TICK = (int)(((WIDTH * HEIGHT) * WORLD_UPDATED_PER_SECOND) / World.TICKS_PER_SECOND);

        private readonly BlockType[,] _blockGrid = new BlockType[HEIGHT, WIDTH];

        public BlockType[,] RawBlockGrid => _blockGrid;

        private ref BlockType BlockTypeAt(int x, int y) => ref _blockGrid[y, x];

        public BlockType GetBlockType(Point point) => GetBlockType(point.X, point.Y);

        public BlockType GetBlockType(int x, int y) => BlockTypeAt(x, y);

        public void SetBlockType(Point point, BlockType blockType) => SetBlockType(point.X, point.Y, blockType);

        public void SetBlockType(int x, int y, BlockType blockType) => BlockTypeAt(x, y) = blockType;

        public int GetTopPosition(int x)
        {
            for (int y = HEIGHT - 1; y >= 0; y--)
            {
                var blockType = GetBlockType(x, y);
                if (!blockType.GetBlock().CanWalkThrough)
                    return y + 1;
            }
            return 0;
        }

        public (BlockType blockType, int y) GetTopBlock(int x)
        {
            for (int y = HEIGHT - 1; y >= 0; y--)
            {
                var blockType = GetBlockType(x, y);
                if (!blockType.GetBlock().CanWalkThrough)
                    return (blockType, y);
            }
            return (BlockType.Air, 0);
        }

        public void Update()
        {
            for (int i = 0; i < BLOCK_UPDATES_PER_TICK; i++)
            {
                // get random point
                var pos = Util.Random.NextPoint(new Point(WIDTH, HEIGHT));
                // update block at that point
                GetBlockType(pos).GetBlock().Update(pos, this);
            }
        }

        public void Draw(AbstractEntity player, BlockHit blockHit, Point mouseBlock, bool withinReach)
        {
            var drawScale = new Vector2(Display.BlockScale);
            // find edge to start drawing
            var visualWidth = (int)Math.Ceiling((double)Display.WindowSize.X / (double)Display.BlockScale) + 4;
            var visualHeight = (int)Math.Ceiling((double)Display.WindowSize.Y / (double)Display.BlockScale) + 4;
            var visualStartX = (int)Math.Floor(player.Position.X - (visualWidth / 2f));
            var visualStartY = (int)Math.Ceiling(player.Position.Y - (visualHeight / 2f)) + 2;
            // fix variables if out of bounds
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
            if (visualWidth >= WIDTH - visualStartX)
                visualWidth = WIDTH - visualStartX;
            if (visualHeight >= HEIGHT - visualStartY)
                visualHeight = HEIGHT - visualStartY;
            // draw visible blocks
            for (int y = 0; y < visualHeight; y++)
            {
                var blockY = y + visualStartY;
                var drawY = (-1 - blockY) * Display.BlockScale;
                for (int x = 0; x < visualWidth; x++)
                {
                    var blockX = x + visualStartX;
                    var blockPos = new Point(blockX, blockY);
                    var blockType = GetBlockType(blockPos);
                    var drawPos = new Vector2(blockX * Display.BlockScale, drawY);
                    var highlighted = blockPos == mouseBlock && withinReach;
                    if (blockType == BlockType.Air && !Debug.HasDebugUpdate(blockPos))
                    {
                        // draw highlight over block mouse is hovering over
                        if (highlighted)
                            Display.DrawOffset(drawPos, drawScale, Colors.BlockHighlightAir);
                        continue;
                    }
                    var block = blockType.GetBlock();
                    Display.DrawOffset(drawPos, drawScale, block.Color, block.Texture);
                    // draw block hit
                    if (blockHit.Position == blockPos)
                    {
                        // TODO draw scaled string
                        var hitsLeft = block.HitsToBreak - blockHit.Hits;
                        if (hitsLeft > 0)
                            Display.DrawOffsetString(FontSize._12, drawPos, hitsLeft.ToString(), Colors.UI_BlockHit, Display.DrawStringWithShadow);
                    }
                    // draw ring over block mouse is hovering over
                    if (highlighted)
                        Display.DrawOffset(drawPos, drawScale, Colors.BlockHighlight, Textures.HighlightRing);
                    // check if block was updated
                    if (Debug.Enabled && Debug.DisplayBlockChecks)
                        // draw faded debug colors over block
                        if (Debug.HasDebugUpdate(blockPos))
                            foreach (var color in Debug.GetDebugColors(blockPos))
                                Display.DrawOffset(drawPos, drawScale, color);
                }
            }
        }
    }
}
