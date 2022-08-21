using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Game.Worlds;

namespace Minicraft.Game.Blocks
{
    public sealed class LeavesBlock : Block
    {
        private static readonly Point[] _checkOffsets = new []
        {
            new Point(-1,  1), new Point(0,  1), new Point(1,  1),
            new Point(-1,  0), /*   center   */  new Point(1,  0),
            new Point(-1, -1), new Point(0, -1), new Point(1, -1),
        };

        public LeavesBlock(string name, Color color, bool canWalkThrough = false, Texture2D texture = null) : base (name, color, canWalkThrough, texture) {}

        public sealed override void Update(Point position, World world)
        {
            // check surrounding blocks for logs
            bool log = false;
            foreach (var offset in _checkOffsets)
            {
                var checkPos = position + offset;
                // test valid position
                if (checkPos.X >= 0 && checkPos.X < World.WIDTH &&
                    checkPos.Y >= 0 && checkPos.Y < World.HEIGHT)
                {
                    // if wood detected
                    if (world.GetBlockType(checkPos) == BlockType.Wood)
                    {
                        // set log flag
                        log = true;
                        // break check loop
                        break;
                    }
                }
            }
            // if no log, remove leaves
            if (!log)
                world.SetBlockType(position, BlockType.Air);
            // base call
            base.Update(position, world);
        }
    }
}
