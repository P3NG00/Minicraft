using Microsoft.Xna.Framework;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.ItemType;

namespace MinicraftGame.Game.BlockType
{
    public sealed class BlockHit
    {
        // TODO consider if this needs to be it's own class any more. probably can be integrated with GameScene

        // TODO store tick count the time the hit happened. once certain amount of seconds/ticks have passed, nullify blockhit
        public Point Position { get; private set; }
        public int Hits { get; private set; }

        public BlockHit(Point position, int hits)
        {
            Position = position;
            Hits = hits;
        }

        public void Hit(Point hitPosition)
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
            var block = Minicraft.World.GetBlock(Position);
            // break block
            if (Hits >= block.HitsToBreak)
            {
                // remove block from world
                Minicraft.World.SetBlock(Position, Blocks.Air);
                // spawn item entity where block broke
                var pos = Position.ToVector2() + new Vector2(0.5f, 0.125f);
                var itemEntity = new ItemEntity(pos, new BlockItem(block));
                Minicraft.World.AddEntity(itemEntity);
                // reset info
                Position = new Point(-1);
                Hits = 0;
            }
        }
    }
}
