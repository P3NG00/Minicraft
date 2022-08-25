using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Minicraft.Font;
using Minicraft.Game.Blocks;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.Inventories
{
    public sealed class Inventory
    {
        public const int SLOTS = 9;
        private const int SLOT_SIZE = 50;

        public static readonly Point HotbarSize = new Point((SLOT_SIZE * SLOTS) + (Util.UI_SPACER * (SLOTS + 1)), SLOT_SIZE + (Util.UI_SPACER * 2));

        private readonly Slot[] _inventory = new Slot[SLOTS];
        private int _activeSlot = 0;

        public Inventory()
        {
            // initialize inventory slots
            for (var i = 0; i < _inventory.Length; i++)
                _inventory[i] = new Slot();
        }

        public List<Slot> GetSlotsOf(BlockType blockType)
        {
            var slots = new List<Slot>();
            foreach (var slot in _inventory)
                if (slot.BlockType == blockType)
                    slots.Add(slot);
            return slots;
        }

        public List<Slot> GetEmptySlots()
        {
            var slots = new List<Slot>();
            foreach (var slot in _inventory)
                if (slot.IsEmpty)
                    slots.Add(slot);
            return slots;
        }

        public void SetActiveSlot(int i) => _activeSlot = i;

        public void Place(World world, Point blockPos)
        {
            var slot = _inventory[_activeSlot];
            if (slot.IsEmpty)
                return;
            if (world.GetBlockType(blockPos) == BlockType.Air)
            {
                world.SetBlockType(blockPos, slot.BlockType);
                slot.Decrement();
            }
        }

        public int? Add(BlockType blockType, int amount = 1)
        {
            if (amount <= 0)
                throw new System.Exception("Amount must be greater than 0");
            if (blockType == BlockType.Air)
                throw new System.Exception("Cannot add air to inventory");
            // add to slots of same type
            var slots = GetSlotsOf(blockType);
            int? amountRemaining = amount;
            foreach (var slot in slots)
            {
                amountRemaining = slot.Add(amountRemaining.Value);
                if (!amountRemaining.HasValue)
                    return null;
            }
            // add to empty slots
            slots = GetEmptySlots();
            foreach (var slot in slots)
            {
                amountRemaining = slot.Set(blockType, amount);
                if (!amountRemaining.HasValue)
                    return null;
                amount = amountRemaining.Value;
            }
            return amountRemaining;
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
            // draw each slot
            for (var i = 0; i < _inventory.Length; i++)
            {
                slot = _inventory[i];
                // if active slot, draw selected border 1 px thick around slot
                if (i == _activeSlot)
                    Display.Draw(drawPos - new Vector2(2), drawSize + new Vector2(4), Colors.HotbarSelected);
                // slot color
                color = Colors.HotbarSlotBackground;
                if (!slot.IsEmpty)
                    color = slot.BlockType.GetBlock().Color;
                Display.Draw(drawPos, drawSize, color);
                // draw string of amount of block in slot
                if (!slot.IsEmpty)
                    Display.DrawStringWithShadow(FontSize._12, drawPos + new Vector2(Util.UI_SPACER, Util.UI_SPACER), slot.Amount.ToString(), Colors.HotbarSlotText);
                drawPos += slotOffset;
            }
        }

        public Slot this[int i] => _inventory[i];
    }
}
