using Microsoft.Xna.Framework;
using Minicraft.Game.Inventories;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Blocks
{
    public struct BlockHit
    {
        public Point Position;
        public int Hits;

        public BlockHit(Point position, int hits)
        {
            Position = position;
            Hits = hits;
        }

        public void Update(World world, Inventory inventory, Point hitPosition)
        {
            // if hit same block
            if (Position == hitPosition)
            {
                // increase hits
                Hits++;
                // get blocktype
                var blockType = world.GetBlockType(hitPosition);
                // break block
                if (Hits >= blockType.GetBlock().HitsToBreak)
                {
                    // add to players inventory
                    inventory.Add(blockType);
                    // remove block from world
                    world.SetBlockType(Position, BlockType.Air);
                    // reset hits
                    Hits = 0;
                }
            }
            // if not same block hit start counting at new position
            else
            {
                Position = hitPosition;
                Hits = 1;
            }
        }
    }
}
