using Microsoft.Xna.Framework;
using Minicraft.Game.Inventories;
using Minicraft.Game.ItemType;
using Minicraft.Game.Worlds;

namespace Minicraft.Game.BlockType
{
    public sealed class BlockHit
    {
        // TODO store tick count the time the hit happened. once certain amount of seconds/ticks have passed, nullify blockhit
        public Point Position { get; private set; }
        public int Hits { get; private set; }

        public BlockHit(Point position, int hits)
        {
            Position = position;
            Hits = hits;
        }

        public void Update(World world, Inventory inventory, Point hitPosition)
        {
            // if hit same block
            if (Position == hitPosition)
                // increase hits
                Hits++;
            // if not same block hit start counting at new position
            else
            {
                Position = hitPosition;
                Hits = 1;
            }
            // get block
            var block = world.GetBlock(hitPosition);
            // break block
            if (Hits >= block.HitsToBreak)
            {
                // add to players inventory
                inventory.Add(new BlockItem(block));
                // remove block from world
                world.SetBlock(Position, Blocks.Air);
                // reset info
                Position = new Point(-1);
                Hits = 0;
            }
        }
    }
}
