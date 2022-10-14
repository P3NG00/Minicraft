using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.Entities;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.ItemType;
using MinicraftGame.Game.Worlds;

namespace MinicraftGame.Game.BlockType
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

        public void Hit(World world, Inventory inventory, Point hitPosition, Action<AbstractEntity> spawnEntityFunc)
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
            var block = world.GetBlock(Position);
            // break block
            if (Hits >= block.HitsToBreak)
            {
                // spawn item entity where block broke
                var pos = Position.ToVector2() + new Vector2(0.5f, 0.125f);
                var itemEntity = new ItemEntity(pos, new BlockItem(block));
                spawnEntityFunc(itemEntity);
                // remove block from world
                world.SetBlock(Position, Blocks.Air);
                // reset info
                Position = new Point(-1);
                Hits = 0;
            }
        }
    }
}
