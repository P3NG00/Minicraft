using Microsoft.Xna.Framework;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.BlockType
{
    public sealed class GrassBlock : Block
    {
        private static readonly Point[] _spreadOffsets = new []
        {
            new Point(-1,  1), new Point(1,  1),
            new Point(-1,  0), new Point(1,  0),
            new Point(-1, -1), new Point(1, -1),
        };

        public GrassBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1) : base(name, hitsToBreak, canWalkThrough, drawData, id) {}

        public sealed override void RandomTick(Point position)
        {
            // if able to spread
            if (position.Y + 1 == World.HEIGHT || Minicraft.World.GetBlock(position + new Point(0, 1)).CanWalkThrough)
            {
                // check random spread position
                var offset = _spreadOffsets.GetRandom();
                var checkPos = position + offset;
                if (checkPos.X >= 0 && checkPos.X < World.WIDTH &&
                    checkPos.Y >= 0 && checkPos.Y < World.HEIGHT)
                {
                    var upPos = checkPos + new Point(0, 1);
                    if (Minicraft.World.GetBlock(checkPos) == Blocks.Dirt && (upPos.Y == World.HEIGHT || Minicraft.World.GetBlock(upPos).CanWalkThrough))
                        Minicraft.World.SetBlock(checkPos, Blocks.Grass);
                }
            }
            // if unable to spread
            else
                Minicraft.World.SetBlock(position, Blocks.Dirt);
            // base call
            base.RandomTick(position);
        }
    }
}
