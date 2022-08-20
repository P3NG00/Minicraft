using Microsoft.Xna.Framework;
using Minicraft.Game.Blocks;
using Minicraft.Utils;

namespace Minicraft.Game.Inventories
{
    public sealed class Inventory
    {
        private const int SLOTS = 9;
        private const int SLOT_SIZE = 50;

        public static readonly Point HotbarSize = new Point((SLOT_SIZE * SLOTS) + (Util.UI_SPACER * (SLOTS + 1)), SLOT_SIZE + (Util.UI_SPACER * 2));

        private readonly Slot[] _inventory = new Slot[SLOTS];
        private int _inventorySlot = 0;

        public Inventory()
        {
            // initialize inventory slots
            for (var i = 0; i < _inventory.Length; i++)
                _inventory[i] = new Slot();
        }

        public void SetSlot(int i) => _inventorySlot = i;

        public void Add(BlockType blockType, int amount = 1)
        {
            int? firstOpenSlot = null;
            // search through inventory slots
            for (int i = 0; i < SLOTS; i++)
            {
                var slot = _inventory[i];
                // if blocktype matches
                if (slot.BlockType == blockType)
                {
                    // increment amount
                    slot.Add();
                    break;
                }
                // if slot is empty
                else if (slot.IsEmpty && !firstOpenSlot.HasValue)
                    // save index of first empty slot
                    firstOpenSlot = i;
            }
            // if there is an empty slot, add block to it
            if (firstOpenSlot.HasValue)
                _inventory[firstOpenSlot.Value].Set(blockType, amount);
        }

        public void Draw()
        {
            // draw bar background
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (HotbarSize.X / 2f), Display.WindowSize.Y - HotbarSize.Y);
            Display.Draw(drawPos, HotbarSize.ToVector2(), Colors.HotbarBackground);
            // draw each slot background
            drawPos += new Vector2(Util.UI_SPACER);
            var drawSize = new Vector2(SLOT_SIZE);
            var slotOffset = new Vector2(Util.UI_SPACER + SLOT_SIZE, 0);
            Slot slot;
            Color color;
            for (var i = 0; i < _inventory.Length; i++)
            {
                slot = _inventory[i];
                color = Colors.HotbarSlotBackground;
                if (!slot.IsEmpty)
                    color = slot.BlockType.GetBlock().Color;
                Display.Draw(drawPos, drawSize, color);
                // draw string of amount of block in slot
                if (!slot.IsEmpty)
                    Display.DrawShadowedString(FontSize._12, drawPos + new Vector2(Util.UI_SPACER, Util.UI_SPACER), slot.Amount.ToString(), Colors.HotbarSlotText);
                drawPos += slotOffset;
            }
        }
    }
}
