using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects.BlockObject
{
    public sealed class GrassBlock : Block
    {
        public GrassBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1) : base(name, hitsToBreak, canWalkThrough, drawData, id) {}

        public sealed override void RandomTick(Point position)
        {
            // check if grass can live
            var upPos = position + new Point(0, 1);
            var upBlock = Minicraft.World.GetBlock(upPos);
            var canGrassLive = upBlock == null || upBlock.CanWalkThrough;
            if (canGrassLive)
            {
                // check random spread position
                var randomX = new[] {-1, 1}.GetRandom();
                var randomY = new[] {-1, 0, 1}.GetRandom();
                var offset = new Point(randomX, randomY);
                var checkPos = position + offset;
                var checkBlock = Minicraft.World.GetBlock(checkPos);
                if (checkBlock != null)
                {
                    Debug.AddGrassSpreadCheck(checkPos);
                    if (checkBlock == Blocks.Dirt)
                    {
                        // check block above
                        upPos = checkPos + new Point(0, 1);
                        upBlock = Minicraft.World.GetBlock(upPos);
                        var canSpread = upBlock == null || upBlock.CanWalkThrough;
                        if (canSpread)
                            Minicraft.World.SetBlock(checkPos, Blocks.Grass);
                    }
                }
            }
            // if unable to live
            else
                Minicraft.World.SetBlock(position, Blocks.Dirt);
            // base call
            base.RandomTick(position);
        }
    }
}
