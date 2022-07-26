using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Game.Data
{
    public sealed class World
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Area { get; private set; }
        public float Gravity { get; private set; }
        public int BlockUpdatesPerTick { get; private set; }

        private Block[][] _blockGrid;

        private World(Block[][] blockGrid, float gravity, int blockUpdatesPerTick)
        {
            _blockGrid = blockGrid;
            Width = _blockGrid[0].Length;
            Height = _blockGrid.Length;
            Area = Width * Height;
            Gravity = gravity;
            BlockUpdatesPerTick = blockUpdatesPerTick;
        }

        public ref Block Block(Point position) => ref _blockGrid[position.Y][position.X];

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
                var pos = new Point(Util.Random.Next(Width), Util.Random.Next(Height));
                Block(pos).Update(pos, this);
            }
        }

        public void Draw(Display display, Entity player)
        {
            var drawScale = display.ShowGrid ? new Vector2(display.BlockScale - 1) : new Vector2(display.BlockScale);
            // find edge to start drawing
            var visualWidth = (int)MathF.Ceiling(display.WindowSize.X / display.BlockScale) + 3;
            var visualHeight = (int)MathF.Ceiling(display.WindowSize.Y / display.BlockScale) + 3;
            var visualStartX = (int)MathF.Floor(player.Position.X - (visualWidth / 2f));
            var visualStartY = (int)MathF.Floor(player.Position.Y - (visualHeight / 2f));
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
                    display.Draw(drawPos, drawScale, Block(new Point(_x, _y)).Color);
                }
            }
        }

        public static World GenerateWorld(Point worldSize, float gravity, int blockUpdatesPerTick)
        {
            // TODO adjust _chunk_width, _height_variation, and _scan_radius variables for different effects
            // create grid of air blocks for modification
            var blockGrid = new Block[worldSize.Y][];
            for (int y = 0; y < worldSize.Y; y++)
            {
                var blockLayer = new Block[worldSize.X];
                Array.Fill(blockLayer, Blocks.Air);
                blockGrid[y] = blockLayer;
            }
            var world = new World(blockGrid, gravity, blockUpdatesPerTick);
            // create height map
            var chunkWidth = 16;
            var relativeWidth = world.Width / chunkWidth;
            var midHeight = world.Height / 2;
            var heightVariation = 32;
            var heightmap = new int[world.Width];
            for (int w = 0; w < relativeWidth; w++)
            {
                var height = midHeight + Util.Random.Next(-heightVariation, heightVariation);
                for (int h = 0; h < chunkWidth; h++)
                    heightmap[(w * chunkWidth) + h] = height;
            }
            // smooth height map
            var scanRadius = 32;
            var heightmapSmooth = new int[world.Width];
            var currentHeights = new int[scanRadius * 2];
            var thirdHeight = (int)(world.Height / 3f);
            int x;
            for (x = 0; x < world.Width; x++)
            {
                // get average height of surrounding area
                for (int scanX = -scanRadius; scanX < scanRadius; scanX++)
                {
                    var _x = x + scanX;
                    currentHeights[scanX + scanRadius] = _x < 0 || _x >= world.Width ? thirdHeight : heightmap[_x];
                }
                heightmapSmooth[x] = (int)currentHeights.Average();
            }
            // place blocks using smoothed height map
            for (x = 0; x < world.Width; x++)
            {
                var heightMax = heightmapSmooth[x] - 1;
                for (int y = 0; y < heightmapSmooth[x]; y++)
                {
                    var block = Blocks.Dirt;
                    if (y == heightMax)
                        block = Blocks.Grass;
                    else if (y < heightMax - 32)
                        block = Blocks.Stone;
                    world.Block(new Point(x, y)) = block;
                }
            }
            // generate trees
            var treeMinSpacing = 6;
            var treeChance = 0.2f;
            var treeMinHeight = 6;
            var treeMaxHeight = 10;
            var branchChance = 0.1f;
            var maxBranchLength = 3;
            for (x = treeMinSpacing; x < world.Width - treeMinSpacing; x++)
            {
                // test tree chance
                if (Util.Random.NextDouble() < treeChance)
                {
                    // generate tree
                    var startY = heightmapSmooth[x];
                    var height = Util.Random.Next(treeMinHeight, treeMaxHeight + 1);
                    var branchDirection = 0;
                    for (int y = startY; y < startY + height; y++)
                    {
                        var setPoint = new Point(x, y);
                        world.Block(setPoint) = Blocks.Wood;
                        // leaves and branches start  2 blocks above ground
                        if (y > startY + 1)
                        {
                            // create leaves on sides of tree
                            foreach (int s in new[] {-1, 1})
                                world.Block(setPoint + new Point(s, 0)) = Blocks.Leaves;
                            // test branch chance
                            if (Util.Random.NextDouble() < branchChance)
                            {
                                // create branch
                                if (branchDirection == 0)
                                    branchDirection = Util.Random.NextBool() ? 1 : -1;
                                else
                                    branchDirection = branchDirection == -1 ? 1 : -1;
                                var branchLength = Util.Random.Next(maxBranchLength) + 1;
                                Point branchPoint = default;
                                for (int i = 0; i < branchLength; i++)
                                {
                                    branchPoint = setPoint + new Point(branchDirection * (i + 1), 0);
                                    // place branch block
                                    world.Block(branchPoint) = Blocks.Wood;
                                    // place leaves surrounding branch
                                    foreach (int o in new[] {-1, 1})
                                        world.Block(branchPoint + new Point(0, o)) = Blocks.Leaves;
                                }
                                // places leaves on end of branch
                                world.Block(branchPoint + new Point(branchDirection, 0)) = Blocks.Leaves;
                            }
                            else
                                branchDirection = 0;
                        }
                    }
                    // place leaves on top of tree
                    world.Block(new Point(x, startY + height)) = Blocks.Leaves;
                    // space trees by minimum amount
                    x += treeMinSpacing;
                }
            }
            // return generated world
            return world;
        }
    }
}
