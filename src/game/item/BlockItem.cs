using Microsoft.Xna.Framework;
using MinicraftGame.Game.BlockType;
using MinicraftGame.Game.Entities.Living;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.Worlds;

namespace MinicraftGame.Game.ItemType
{
    public sealed class BlockItem : Item
    {
        private readonly Block _block;

        public BlockItem(Block block) : base(block.Name, block.DrawData) => _block = block;

        public sealed override void Use(World world, Slot slot, PlayerEntity player, Point blockPosition)
        {
            if (world.GetBlock(blockPosition) == Blocks.Air)
            {
                var inPlayer = player.GetSides().Contains(blockPosition);
                if (!inPlayer)
                {
                    world.SetBlock(blockPosition, _block);
                    slot.Decrement();
                }
            }
        }
    }
}
