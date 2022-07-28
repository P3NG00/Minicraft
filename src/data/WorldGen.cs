using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Game.Data
{
    public sealed class WorldGen
    {
        private const int CHUNK_WIDTH = 16;
        private const int CHUNK_HEIGHT_VARIATION_RADIUS = 32;
        private const int SMOOTH_SCAN_RADIUS = 32;
        private const int TREE_SPACING_MIN = 5;
        private const int TREE_HEIGHT_MIN = 8;
        private const int TREE_HEIGHT_MAX = 24;
        private const int BRANCH_LENGTH_MAX = 5;
        private const float TREE_CHANCE = 0.2f;
        private const float BRANCH_CHANCE = 0.15f;

        public static World GenerateWorld(Point worldSize, float gravity, int blockUpdatesPerTick)
        {
            int x;
            int y;
            // create grid of air blocks for modification
            var blockGrid = new Block[worldSize.Y, worldSize.X];
            for (y = 0; y < worldSize.Y; y++)
                for (x = 0; x < worldSize.X; x++)
                    blockGrid[y, x] = Blocks.Air;
            var world = new World(blockGrid, gravity, blockUpdatesPerTick);
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
            for (x = TREE_SPACING_MIN; x < world.Width - TREE_SPACING_MIN; x++)
            {
                // test tree chance
                if (Util.Random.TestChance(TREE_CHANCE))
                {
                    // generate tree
                    var startY = heightmapSmooth[x];
                    var height = Util.Random.Next(TREE_HEIGHT_MIN, TREE_HEIGHT_MAX + 1);
                    var branchDirection = 0;
                    for (y = startY; y < startY + height; y++)
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
                            if (Util.Random.TestChance(BRANCH_CHANCE))
                            {
                                // create branch
                                if (branchDirection == 0)
                                    branchDirection = Util.Random.NextBool() ? 1 : -1;
                                else
                                    branchDirection = branchDirection == -1 ? 1 : -1;
                                var branchLength = Util.Random.Next(BRANCH_LENGTH_MAX) + 1;
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
                    x += TREE_SPACING_MIN;
                }
            }
            // return generated world
            return world;
        }
    }
}
