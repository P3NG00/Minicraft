using Microsoft.Xna.Framework;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.BlockType
{
    public sealed class LeavesBlock : Block
    {
        public LeavesBlock(string name, int hitsToBreak, bool canWalkThrough, DrawData drawData, int id = -1) : base(name, hitsToBreak, canWalkThrough, drawData, id) {}

        public sealed override void RandomTick(Point position)
        {
            // check surrounding blocks for logs
            bool foundWood = false;
            for (int y = -1; y <= 1 && !foundWood; y++)
            {
                for (int x = -1; x <= 1 && !foundWood; x++)
                {
                    // skip checking self
                    if (x == 0 && y == 0)
                        continue;
                    var checkPos = position + new Point(x, y);
                    Debug.AddWoodCheck(checkPos);
                    var checkBlock = Minicraft.World.GetBlock(checkPos);
                    if (checkBlock == Blocks.Wood)
                        foundWood = true;
                }
            }
            // if no log, remove leaves
            if (!foundWood)
                Minicraft.World.SetBlock(position, Blocks.Air);
            // base call
            base.RandomTick(position);
        }
    }
}
