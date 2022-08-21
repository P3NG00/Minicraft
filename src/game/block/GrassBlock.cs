using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public GrassBlock(string name, Color color, bool canWalkThrough = false, Texture2D texture = null) : base(name, color, canWalkThrough, texture) {}

        public sealed override void Update(Point position, World world)
        {
            // if able to spread
            if (position.Y + 1 == World.HEIGHT || world.GetBlockType(position + new Point(0, 1)).GetBlock().CanWalkThrough)
            {
                // check random spread position
                var offset = _spreadOffsets.GetRandom();
                var checkPos = position + offset;
                if (checkPos.X >= 0 && checkPos.X < World.WIDTH &&
                    checkPos.Y >= 0 && checkPos.Y < World.HEIGHT)
                {
                    var upPos = checkPos + new Point(0, 1);
                    if (world.GetBlockType(checkPos) == BlockType.Dirt && (upPos.Y == World.HEIGHT || world.GetBlockType(upPos).GetBlock().CanWalkThrough))
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
