using Microsoft.Xna.Framework;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.BlockType
{
    public sealed class GrassBlock : Block
    {
        public GrassBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1) : base(name, hitsToBreak, canWalkThrough, drawData, id) {}

        public sealed override void RandomTick(Point position)
        {
            // if able to spread
            if (position.Y + 1 == World.HEIGHT || Minicraft.World.GetBlock(position + new Point(0, 1)).CanWalkThrough)
            {
                // check random spread position
                var randomX = new[] {-1, 1}.GetRandom();
                var randomY = new[] {-1, 0, 1}.GetRandom();
                var offset = new Point(randomX, randomY);
                var checkPos = position + offset;
                // debug grass spread check
                Debug.AddGrassSpreadCheck(checkPos);
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
