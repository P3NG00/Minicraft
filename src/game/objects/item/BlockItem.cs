using Microsoft.Xna.Framework;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.Objects.BlockObject;

namespace MinicraftGame.Game.Objects.ItemObject
{
    public sealed class BlockItem : Item
    {
        private readonly Block _block;

        public Block Block => _block;

        public BlockItem(Block block) : base(block.Name, block.DrawData, -1) => _block = block;

        public sealed override void Use(Slot slot, Point blockPosition)
        {
            if (Minicraft.World.GetBlock(blockPosition) == Blocks.Air)
            {
                var inPlayer = Minicraft.Player.GetSides().Contains(blockPosition);
                if (!inPlayer)
                {
                    Minicraft.World.SetBlock(blockPosition, _block);
                    slot.Decrement();
                }
            }
            else
                base.Use(slot, blockPosition);
        }
    }
}
