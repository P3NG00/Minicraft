using System;
using System.Linq;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.Objects.BlockObject;
using MinicraftGame.Utils;
using SimplexNoise;

namespace MinicraftGame.Game.Worlds.Generation
{
    public static class WorldGen
    {
        public static World GenerateWorld(WorldGenSettings settings)
        {
            // create world of air blocks for modification
            int x, y;
            var world = new World();
            for (y = 0; y < World.HEIGHT; y++)
                for (x = 0; x < World.WIDTH; x++)
                    world.SetBlock(x, y, Blocks.Air);
            // create random height map
            var relativeWidth = World.WIDTH / settings.ChunkWidth;
            var midHeight = World.HEIGHT / 2;
            var heightmap = new int[World.WIDTH];
            for (int w = 0; w < relativeWidth; w++)
            {
                var height = midHeight + Util.Random.Next(-settings.ChunkHeightVariationRadius, settings.ChunkHeightVariationRadius);
                for (int h = 0; h < settings.ChunkWidth; h++)
                    heightmap[(w * settings.ChunkWidth) + h] = height;
            }
            // smooth height map
            var heightmapSmooth = new int[World.WIDTH];
            var currentHeights = new int[settings.SmoothScanRadius * 2];
            var thirdHeight = (int)(World.HEIGHT / 3f);
            for (x = 0; x < World.WIDTH; x++)
            {
                // get average height of surrounding area
                for (int scanX = -settings.SmoothScanRadius; scanX < settings.SmoothScanRadius; scanX++)
                {
                    var _x = x + scanX;
                    var inBounds = _x >= 0 && _x < World.WIDTH;
                    var height = inBounds ? heightmap[_x] : thirdHeight;
                    currentHeights[scanX + settings.SmoothScanRadius] = height;
                }
                heightmapSmooth[x] = (int)Math.Round(currentHeights.Average());
            }
            // place ground using smoothed height map
            for (x = 0; x < World.WIDTH; x++)
            {
                var heightMax = heightmapSmooth[x] - 1;
                for (y = 0; y < heightmapSmooth[x]; y++)
                {
                    var isStone = y <= heightMax - settings.StoneOffset;
                    var block = isStone ? Blocks.Stone : Blocks.Dirt;
                    world.SetBlock(x, y, block);
                }
            }
            // create noise map for caves
            Noise.Seed = Util.Random.Next(int.MinValue, int.MaxValue);
            var noiseMap = Noise.Calc2D(World.WIDTH, World.HEIGHT, (float)settings.CaveNoiseScale);
            // iterate through noisemap values and remove blocks at cutoff point
            for (y = 0; y < World.HEIGHT; y++)
                for (x = 0; x < World.WIDTH; x++)
                    if ((decimal)noiseMap[x, y] < settings.CaveNoiseCutoff)
                        world.SetBlock(x, y, Blocks.Air);
            // place grass on top-most dirt blocks
            for (x = 0; x < World.WIDTH; x++)
            {
                var topBlock = world.GetTopBlock(x);
                if (topBlock.block == Blocks.Dirt)
                    world.SetBlock(x, topBlock.y, Blocks.Grass);
            }
            // generate trees on surface grass
            for (x = settings.TreeSpacingMin; x < World.WIDTH - settings.TreeSpacingMin; x++)
            {
                var topBlock = world.GetTopBlock(x);
                // test tree chance
                if (topBlock.block == Blocks.Grass && settings.TreeChance.Value.TestChance())
                {
                    // generate tree
                    var startY = topBlock.y + 1;
                    var height = Util.Random.Next(settings.TreeHeightMin, settings.TreeHeightMax + 1);
                    var branchDirection = 0;
                    for (y = startY; y < startY + height && y < World.HEIGHT; y++)
                    {
                        var setPoint = new Point(x, y);
                        world.SetBlock(setPoint, Blocks.Wood);
                        // leaves and branches start 2 blocks above ground
                        if (y >= startY + 2)
                        {
                            // create leaves on sides of tree
                            foreach (int s in new[] {-1, 1})
                            {
                                // get reference of side block
                                var sidePoint = setPoint + new Point(s, 0);
                                if (sidePoint.X >= 0 && sidePoint.X < World.WIDTH && world.GetBlock(sidePoint) == Blocks.Air)
                                    world.SetBlock(sidePoint, Blocks.Leaves);
                            }
                            // test branch chance
                            if (settings.BranchChance.Value.TestChance())
                            {
                                // create branch
                                branchDirection = (branchDirection == 0 ? Util.Random.NextBool() : branchDirection == -1) ? 1 : -1;
                                var branchLength = Util.Random.Next(settings.BranchLengthMax) + 1;
                                Point branchPoint = default;
                                for (int i = 0; i < branchLength; i++)
                                {
                                    // find new branch point
                                    branchPoint = setPoint + new Point(branchDirection * (i + 1), 0);
                                    // break placement if invalid position
                                    if (branchPoint.X < 0 || branchPoint.X >= World.WIDTH)
                                        break;
                                    // get reference of block at that position
                                    var branchBlock = world.GetBlock(branchPoint);
                                    // replace if valid
                                    if (branchBlock == Blocks.Air || branchBlock == Blocks.Leaves)
                                        world.SetBlock(branchPoint, Blocks.Wood);
                                    // place leaves surrounding branch
                                    foreach (int o in new[] {-1, 1})
                                    {
                                        // get reference of block at new branch point
                                        var sidePoint = branchPoint + new Point(0, o);
                                        // replace if valid
                                        if (sidePoint.Y < World.HEIGHT && world.GetBlock(sidePoint) == Blocks.Air)
                                            world.SetBlock(sidePoint, Blocks.Leaves);
                                    }
                                }
                                // get position of end of branch
                                var endPoint = branchPoint + new Point(branchDirection, 0);
                                // place leaves at end of branch if valid
                                if (endPoint.X >= 0 && endPoint.X < World.WIDTH && world.GetBlock(endPoint) == Blocks.Air)
                                    world.SetBlock(endPoint, Blocks.Leaves);
                            }
                            else
                                // reset branch direction
                                branchDirection = 0;
                        }
                    }
                    // place leaves on top of tree
                    y = startY + height;
                    if (y < World.HEIGHT)
                        world.SetBlock(x, y, Blocks.Leaves);
                    // space trees by minimum amount
                    x += settings.TreeSpacingMin;
                }
            }
            // return generated world
            return world;
        }
    }
}
