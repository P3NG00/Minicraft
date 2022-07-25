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
                if (!_block.IsAir)
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
            var relativeWidth = (int)(world.Width / chunkWidth);
            var midHeight = (int)(world.Height / 2);
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
            for (int x = 0; x < world.Width; x++)
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
            for (int x = 0; x < world.Width; x++)
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
            // TODO generate trees across surface of map
            // return generated world
            return world;
        }
    }
}
