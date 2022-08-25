using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Minicraft.Game.Blocks;
using Minicraft.Utils;
using SimplexNoise;

namespace Minicraft.Game.Worlds
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
            // create world of air blocks for modification
            int x, y;
            var world = new World();
            for (y = 0; y < World.HEIGHT; y++)
                for (x = 0; x < World.WIDTH; x++)
                    world.SetBlockType(x, y, BlockType.Air);
            // create random height map
            var relativeWidth = World.WIDTH / CHUNK_WIDTH;
            var midHeight = World.HEIGHT / 2;
            var heightmap = new int[World.WIDTH];
            for (int w = 0; w < relativeWidth; w++)
            {
                var height = midHeight + Util.Random.Next(-CHUNK_HEIGHT_VARIATION_RADIUS, CHUNK_HEIGHT_VARIATION_RADIUS);
                for (int h = 0; h < CHUNK_WIDTH; h++)
                    heightmap[(w * CHUNK_WIDTH) + h] = height;
            }
            // smooth height map
            var heightmapSmooth = new int[World.WIDTH];
            var currentHeights = new int[SMOOTH_SCAN_RADIUS * 2];
            var thirdHeight = (int)(World.HEIGHT / 3f);
            for (x = 0; x < World.WIDTH; x++)
            {
                // get average height of surrounding area
                for (int scanX = -SMOOTH_SCAN_RADIUS; scanX < SMOOTH_SCAN_RADIUS; scanX++)
                {
                    var _x = x + scanX;
                    var inBounds = _x >= 0 && _x < World.WIDTH;
                    var height = inBounds ? heightmap[_x] : thirdHeight;
                    currentHeights[scanX + SMOOTH_SCAN_RADIUS] = height;
                }
                heightmapSmooth[x] = (int)Math.Round(currentHeights.Average());
            }
            // place ground using smoothed height map
            for (x = 0; x < World.WIDTH; x++)
            {
                var heightMax = heightmapSmooth[x] - 1;
                for (y = 0; y < heightmapSmooth[x]; y++)
                {
                    var isStone = y <= heightMax - STONE_OFFSET;
                    var block = isStone ? BlockType.Stone : BlockType.Dirt;
                    world.SetBlockType(x, y, block);
                }
            }
            // create noise map for caves
            Noise.Seed = Util.Random.Next(int.MinValue, int.MaxValue);
            var noiseMap = Noise.Calc2D(World.WIDTH, World.HEIGHT, CAVE_NOISE_SCALE);
            // iterate through noisemap values and remove blocks at cutoff point
            for (y = 0; y < World.HEIGHT; y++)
                for (x = 0; x < World.WIDTH; x++)
                    if (noiseMap[x, y] < CAVE_NOISE_CUTOFF)
                        world.SetBlockType(x, y, BlockType.Air);
            // place grass on top-most dirt blocks
            for (x = 0; x < World.WIDTH; x++)
            {
                var topBlock = world.GetTop(x);
                if (topBlock.block == BlockType.Dirt)
                    world.SetBlockType(x, topBlock.y, BlockType.Grass);
            }
            // generate trees on surface grass
            for (x = TREE_SPACING_MIN; x < World.WIDTH - TREE_SPACING_MIN; x++)
            {
                var topBlock = world.GetTop(x);
                // test tree chance
                if (topBlock.block == BlockType.Grass && TREE_CHANCE.TestChance())
                {
                    // generate tree
                    var startY = topBlock.y + 1;
                    var height = Util.Random.Next(TREE_HEIGHT_MIN, TREE_HEIGHT_MAX + 1);
                    var branchDirection = 0;
                    for (y = startY; y < startY + height; y++)
                    {
                        var setPoint = new Point(x, y);
                        world.SetBlockType(setPoint, BlockType.Wood);
                        // leaves and branches start 2 blocks above ground
                        if (y >= startY + 2)
                        {
                            // create leaves on sides of tree
                            foreach (int s in new[] {-1, 1})
                            {
                                // get reference of side block
                                var sidePoint = setPoint + new Point(s, 0);
                                if (world.GetBlockType(sidePoint) == BlockType.Air)
                                    world.SetBlockType(sidePoint, BlockType.Leaves);
                            }
                            // test branch chance
                            if (BRANCH_CHANCE.TestChance())
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
                                    if (branchPoint.X < 0 || branchPoint.X >= World.WIDTH)
                                        break;
                                    // get reference of block at that position
                                    var branchBlock = world.GetBlockType(branchPoint);
                                    // replace if valid
                                    if (branchBlock == BlockType.Air || branchBlock == BlockType.Leaves)
                                        world.SetBlockType(branchPoint, BlockType.Wood);
                                    // place leaves surrounding branch
                                    foreach (int o in new[] {-1, 1})
                                    {
                                        // get reference of block at new branch point
                                        var sidePoint = branchPoint + new Point(0, o);
                                        // replace if valid
                                        if (world.GetBlockType(sidePoint) == BlockType.Air)
                                            world.SetBlockType(sidePoint, BlockType.Leaves);
                                    }
                                }
                                // get position of end of branch
                                var endPoint = branchPoint + new Point(branchDirection, 0);
                                // place leaves at end of branch if valid
                                if (endPoint.X >= 0 && endPoint.X < World.WIDTH && world.GetBlockType(endPoint) == BlockType.Air)
                                        world.SetBlockType(endPoint, BlockType.Leaves);
                            }
                            else
                                // reset branch direction
                                branchDirection = 0;
                        }
                    }
                    // place leaves on top of tree
                    world.SetBlockType(x, startY + height, BlockType.Leaves);
                    // space trees by minimum amount
                    x += TREE_SPACING_MIN;
                }
            }
            // return generated world
            return world;
        }
    }
}
