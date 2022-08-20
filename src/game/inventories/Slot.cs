using Minicraft.Game.Blocks;

namespace Minicraft.Game.Inventories
{
    public class Slot
    {
        public BlockType BlockType { get; private set; }
        public int Amount { get; private set; }

        public bool IsEmpty => BlockType == BlockType.Air || Amount <= 0;

        public Slot() => Set(BlockType.Air, 0);

        public void Add(int amount = 1) => Amount += amount;

        public void Set(BlockType blockType, int amount)
        {
            BlockType = blockType;
            Amount = amount;
        }
    }
}
