using Microsoft.Xna.Framework;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Blocks
{
    public sealed class GrassBlock : Block
    {
        private static readonly Point[] _spreadOffsets = new []
        {
            new Point(-1,  1), new Point(1,  1),
            new Point(-1,  0), new Point(1,  0),
            new Point(-1, -1), new Point(1, -1),
        };

        public GrassBlock(string name, Color color, bool canWalkThrough = false) : base(name, color, canWalkThrough) {}

        public sealed override void Update(Point position, World world)
        {
            // if able to spread
            if (position.Y + 1 == world.Height || world.GetBlockType(position + new Point(0, 1)).GetBlock().CanWalkThrough)
            {
                // check random spread position
                var offset = _spreadOffsets.GetRandom();
                var checkPos = position + offset;
                if (checkPos.X >= 0 && checkPos.X < world.Width &&
                    checkPos.Y >= 0 && checkPos.Y < world.Height)
                {
                    var upPos = checkPos + new Point(0, 1);
                    if (world.GetBlockType(checkPos) == BlockType.Dirt && (upPos.Y == world.Height || world.GetBlockType(upPos).GetBlock().CanWalkThrough))
                        world.SetBlockType(checkPos, BlockType.Grass);
                }
            }
            // if unable to spread
            else
                world.SetBlockType(position, BlockType.Dirt);
            // base call
            base.Update(position, world);
        }
    }
}
