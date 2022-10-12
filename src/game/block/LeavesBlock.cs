using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.BlockType
{
    public sealed class LeavesBlock : Block
    {
        private static readonly Point[] _checkOffsets = new []
        {
            new Point(-1,  1), new Point(0,  1), new Point(1,  1),
            new Point(-1,  0), /*   center   */  new Point(1,  0),
            new Point(-1, -1), new Point(0, -1), new Point(1, -1),
        };

        public LeavesBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData) : base(name, hitsToBreak, canWalkThrough, drawData) {}

        public sealed override void Update(World world, Point position)
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
                    if (world.GetBlock(checkPos) == Blocks.Wood)
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
                world.SetBlock(position, Blocks.Air);
            // base call
            base.Update(world, position);
        }
    }
}
