using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MinicraftGame.Font;
using MinicraftGame.Game.Entities;
using MinicraftGame.Game.Objects.BlockObject;
using MinicraftGame.Texture;
using MinicraftGame.Utils;
using System.Linq;

namespace MinicraftGame.Game.Worlds
{
    public sealed class World
    {
        public const float WORLD_UPDATED_PER_SECOND = 1f / (float)Minicraft.TICKS_PER_SECOND;
        public const float GRAVITY = 10f;
        public const int WIDTH = 1024;
        public const int HEIGHT = 512;
        public const float TICK_STEP = 1f / Minicraft.TICKS_PER_SECOND;
        private const int BLOCK_UPDATES_PER_TICK = (int)(((WIDTH * HEIGHT) * WORLD_UPDATED_PER_SECOND) / Minicraft.TICKS_PER_SECOND);

        // TODO make grid hold only id of blocks
        private readonly Block[,] _blockGrid = new Block[HEIGHT, WIDTH];
        // TODO add 2nd layer for 'background' tiles. make dirt, stone, and wood backgrounds. dirt and stone will generate on its respective block and the back layer will stay when caves are removed from the foreground
        private readonly List<AbstractEntity> _entityList = new();

        public int EntityCount => _entityList.Count;

        public Block[,] RawBlockGrid => _blockGrid;

        private ref Block BlockAt(int x, int y) => ref _blockGrid[y, x];

        public Block GetBlock(Point point) => GetBlock(point.X, point.Y);

        public Block GetBlock(int x, int y)
        {
            if (World.IsValidPosition(x, y))
                return BlockAt(x, y);
            return null;
        }

        public bool SetBlock(Point point, Block block) => SetBlock(point.X, point.Y, block);

        public bool SetBlock(int x, int y, Block block)
        {
            if (World.IsValidPosition(x, y))
            {
                BlockAt(x, y) = block;
                return true;
            }
            return false;
        }

        public Point GetSpawnPosition()
        {
            var x = (int)(World.WIDTH / 2f);
            var y0 = GetTopBlock(x - 1).y;
            var y1 = GetTopBlock(x).y;
            var y = Math.Max(y0, y1) + 1;
            return new(x, y);
        }

        public (Block block, int y) GetTopBlock(int x)
        {
            for (int y = HEIGHT - 1; y >= 0; y--)
            {
                var block = GetBlock(x, y);
                if (!block.CanWalkThrough)
                    return (block, y);
            }
            return (Blocks.Air, 0);
        }

        public void AddEntity(AbstractEntity entity) => _entityList.Add(entity);

        public void Tick()
        {
            var worldSize = new Point(WIDTH, HEIGHT);
            for (int i = 0; i < BLOCK_UPDATES_PER_TICK; i++)
            {
                // get random point
                var pos = Util.Random.NextPoint(worldSize);
                // update block at that point
                GetBlock(pos).RandomTick(pos);
            }
        }

        // ticks then removes dead npcs
        public void TickEntities() => _entityList.RemoveAll(entity =>
        {
            entity.Tick();
            return !entity.Alive;
        });

        public void Draw(Point blockHitPos, int blockHits, Point mouseBlock, bool withinReach)
        {
            var drawScale = new Vector2(Display.BlockScale);
            // find edge to start drawing
            var visualWidth = (int)Math.Ceiling((double)Display.WindowSize.X / (double)Display.BlockScale) + 2;
            var visualHeight = (int)Math.Ceiling((double)Display.WindowSize.Y / (double)Display.BlockScale) + 2;
            var visualStartX = (int)Math.Floor(Minicraft.Player.Center.X - (visualWidth / 2f));
            var visualStartY = (int)Math.Floor(Minicraft.Player.Center.Y - (visualHeight / 2f));
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
                    var drawX = blockX * Display.BlockScale;
                    var blockPos = new Point(blockX, blockY);
                    var block = GetBlock(blockPos);
                    var drawPos = new Vector2(drawX, drawY);
                    // draw block
                    if (block != Blocks.Air)
                        Display.DrawOffset(drawPos, drawScale, block.DrawData);
                    // draw highlight
                    var highlighted = blockPos == mouseBlock && withinReach;
                    if (highlighted)
                    {
                        DrawData drawData;
                        var isAir = block == Blocks.Air;
                        if (isAir)
                            drawData = new(color: Colors.BlockHighlightAir);
                        else
                            drawData = new(Textures.HighlightRing, Colors.BlockHighlight);
                        Display.DrawOffset(drawPos, drawScale, drawData);
                    }
                    // draw blockhit string
                    if (blockHitPos == blockPos)
                    {
                        var hitsLeft = block.HitsToBreak - blockHits;
                        if (hitsLeft > 0)
                        {
                            // TODO figure out weirdness in pixel position
                            var stringScale = new Vector2((float)Display.BlockScale / (float)(Textures.SIZE * 2));
                            Display.DrawOffsetString(FontSize._12, drawPos, hitsLeft.ToString(), Colors.UI_BlockHit, stringScale, drawStringFunc: Display.DrawStringWithShadow);
                        }
                    }
                    // draw debug updates
                    if (Debug.Enabled && Debug.DisplayBlockChecks && Debug.HasDebugUpdate(blockPos))
                        foreach (var color in Debug.GetDebugColors(blockPos))
                            Display.DrawOffset(drawPos, drawScale, new(color: color));
                }
            }
        }

        public void DrawEntities()
        {
            foreach (var entity in _entityList)
                entity.Draw();
        }

        public List<AbstractEntity> GetEntitiesOfType(Type type) => _entityList.Where(entity => entity.GetType() == type).ToList();

        public List<AbstractEntity> GetEntitiesOfType<T>() => GetEntitiesOfType(typeof(T));

        public void Interact(Point blockPos) => GetBlock(blockPos).Interact(blockPos);

        public static bool IsValidPosition(Point position) => IsValidPosition(position.X, position.Y);

        public static bool IsValidPosition(int x, int y) => x >= 0 && x < WIDTH && y >= 0 && y < HEIGHT;
    }
}
