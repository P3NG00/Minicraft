using Microsoft.Xna.Framework;
using MinicraftGame.Game.Worlds;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.BlockType
{
    public sealed class LeavesBlock : Block
    {
        public LeavesBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1) : base(name, hitsToBreak, canWalkThrough, drawData, id) {}

        public sealed override void RandomTick(Point position)
        {
            // check surrounding blocks for logs
            bool log = false;
            for (int y = -1; y <= 1 && !log; y++)
            {
                for (int x = -1; x <= 1 && !log; x++)
                {
                    // skip checking self
                    if (x == 0 && y == 0)
                        continue;
                    // test valid position
                    var checkPos = position + new Point(x, y);
                    // debug wood check
                    Debug.AddWoodCheck(checkPos);
                    if (checkPos.X >= 0 && checkPos.X < World.WIDTH &&
                        checkPos.Y >= 0 && checkPos.Y < World.HEIGHT)
                    {
                        // if wood detected
                        if (Minicraft.World.GetBlock(checkPos) == Blocks.Wood)
                        {
                            // set log flag
                            log = true;
                        }
                    }
                }
            }
            // if no log, remove leaves
            if (!log)
                Minicraft.World.SetBlock(position, Blocks.Air);
            // base call
            base.RandomTick(position);
        }
    }
}
