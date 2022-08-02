using System;
using System.Linq;
using Microsoft.Xna.Framework;
using SimplexNoise;

namespace Game.Data
{
    public static class WorldGen
    {
        private const int CHUNK_WIDTH = 16;
        private const int CHUNK_HEIGHT_VARIATION_RADIUS = 32;
        private const int STONE_OFFSET = 32;
        private const int SMOOTH_SCAN_RADIUS = 32;
        private const int TREE_SPACING_MIN = 5;
        private const int TREE_HEIGHT_MIN = 8;
        private const int TREE_HEIGHT_MAX = 24;
        private const int BRANCH_LENGTH_MAX = 5;
        private const float TREE_CHANCE = 0.2f;
        private const float BRANCH_CHANCE = 0.15f;
        private const float CAVE_NOISE_SCALE = 0.02f;
        private const float CAVE_NOISE_CUTOFF = 64f;

        public static World GenerateWorld()
        {
            int x, y;
            // create grid of air blocks for modification
            var worldSize = World.DefaultSize;
            var blockGrid = new Block[worldSize.Y, worldSize.X];
            for (y = 0; y < worldSize.Y; y++)
                for (x = 0; x < worldSize.X; x++)
                    blockGrid[y, x] = Blocks.Air;
            var world = new World(blockGrid);
            // create height map
            var relativeWidth = world.Width / CHUNK_WIDTH;
            var midHeight = world.Height / 2;
            var heightmap = new int[world.Width];
            for (int w = 0; w < relativeWidth; w++)
            {
                var height = midHeight + Util.Random.Next(-CHUNK_HEIGHT_VARIATION_RADIUS, CHUNK_HEIGHT_VARIATION_RADIUS);
                for (int h = 0; h < CHUNK_WIDTH; h++)
                    heightmap[(w * CHUNK_WIDTH) + h] = height;
            }
            // smooth height map
            var heightmapSmooth = new int[world.Width];
            var currentHeights = new int[SMOOTH_SCAN_RADIUS * 2];
            var thirdHeight = (int)(world.Height / 3f);
            for (x = 0; x < world.Width; x++)
            {
                // get average height of surrounding area
                for (int scanX = -SMOOTH_SCAN_RADIUS; scanX < SMOOTH_SCAN_RADIUS; scanX++)
                {
                    var _x = x + scanX;
                    currentHeights[scanX + SMOOTH_SCAN_RADIUS] = _x < 0 || _x >= world.Width ? thirdHeight : heightmap[_x];
                }
                heightmapSmooth[x] = (int)Math.Round(currentHeights.Average());
            }
            // place blocks using smoothed height map
            for (x = 0; x < world.Width; x++)
            {
                var heightMax = heightmapSmooth[x] - 1;
                for (y = 0; y < heightmapSmooth[x]; y++)
                    world.Block(new Point(x, y)) = y <= heightMax - STONE_OFFSET ? Blocks.Stone : Blocks.Dirt;
            }
            // generate caves
            Noise.Seed = Util.Random.Next(int.MinValue, int.MaxValue);
            var noiseMap = Noise.Calc2D(world.Width, world.Height, CAVE_NOISE_SCALE);
            for (y = 0; y < world.Height; y++)
                for (x = 0; x < world.Width; x++)
                    if (noiseMap[x, y] < CAVE_NOISE_CUTOFF)
                        world.Block(new Point(x, y)) = Blocks.Air;
            // place grass on top-most dirt blocks
            for (x = 0; x < world.Width; x++)
            {
                var topBlock = world.GetTopBlock(x);
                if (topBlock.block == Blocks.Dirt)
                    world.Block(new Point(x, topBlock.y)) = Blocks.Grass;
            }
            // generate trees on on surface grass
            for (x = TREE_SPACING_MIN; x < world.Width - TREE_SPACING_MIN; x++)
            {
                var topBlock = world.GetTopBlock(x);
                // test tree chance
                if (topBlock.block == Blocks.Grass && Util.Random.TestChance(TREE_CHANCE))
                {
                    // generate tree
                    var startY = topBlock.y + 1;
                    var height = Util.Random.Next(TREE_HEIGHT_MIN, TREE_HEIGHT_MAX + 1);
                    var branchDirection = 0;
                    for (y = startY; y < startY + height; y++)
                    {
                        var setPoint = new Point(x, y);
                        world.Block(setPoint) = Blocks.Wood;
                        // leaves and branches start 2 blocks above ground
                        if (y >= startY + 2)
                        {
                            // create leaves on sides of tree
                            foreach (int s in new[] {-1, 1})
                            {
                                // get reference of side block
                                ref Block sideBlock = ref world.Block(setPoint + new Point(s, 0));
                                if (sideBlock == Blocks.Air)
                                    sideBlock = Blocks.Leaves;
                            }
                            // test branch chance
                            if (Util.Random.TestChance(BRANCH_CHANCE))
                            {
                                // create branch
                                branchDirection = (branchDirection == 0 ? Util.Random.NextBool() : branchDirection == -1) ? 1 : -1;
                                var branchLength = Util.Random.Next(BRANCH_LENGTH_MAX) + 1;
                                Point branchPoint = default;
                                for (int i = 0; i < branchLength; i++)
                                {
                                    // find new branch point
                                    branchPoint = setPoint + new Point(branchDirection * (i + 1), 0);
                                    // break placement if invalid position
                                    if (branchPoint.X < 0 || branchPoint.X >= world.Width)
                                        break;
                                    // get reference of block at that position
                                    ref Block branchBlock = ref world.Block(branchPoint);
                                    // replace if valid
                                    if (branchBlock == Blocks.Air || branchBlock == Blocks.Leaves)
                                        branchBlock = Blocks.Wood;
                                    // place leaves surrounding branch
                                    foreach (int o in new[] {-1, 1})
                                    {
                                        // get reference of block at new branch point
                                        ref Block leafBlock = ref world.Block(branchPoint + new Point(0, o));
                                        // replace if valid
                                        if (leafBlock == Blocks.Air)
                                            leafBlock = Blocks.Leaves;
                                    }
                                }
                                // get position of end of branch
                                var endPoint = branchPoint + new Point(branchDirection, 0);
                                // test position
                                if (endPoint.X >= 0 && endPoint.X < world.Width)
                                {
                                    // get reference of block at branch endpoint
                                    ref Block endBlock = ref world.Block(endPoint);
                                    // replace if valid
                                    if (endBlock == Blocks.Air)
                                        endBlock = Blocks.Leaves;
                                }
                            }
                            else
                                // reset branch direction
                                branchDirection = 0;
                        }
                    }
                    // place leaves on top of tree
                    world.Block(new Point(x, startY + height)) = Blocks.Leaves;
                    // space trees by minimum amount
                    x += TREE_SPACING_MIN;
                }
            }
            // return generated world
            return world;
        }
    }
}
