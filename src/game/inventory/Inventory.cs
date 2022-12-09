using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MinicraftGame.Font;
using MinicraftGame.Game.Objects.ItemObject;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Inventories
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

        public List<Slot> GetSlotsOf(Item item) => GetSlots(slot => slot.Item == item);

        public List<Slot> GetEmptySlots() => GetSlots(slot => slot.IsEmpty);

        private List<Slot> GetSlots(Predicate<Slot> predicate)
        {
            var slots = new List<Slot>();
            foreach (var slot in _inventory)
                if (predicate(slot))
                    slots.Add(slot);
            return slots;
        }

        public void SetActiveSlot(int i) => _activeSlot = i;

        public void Use(Point blockPosition)
        {
            var slot = _inventory[_activeSlot];
            if (!slot.IsEmpty)
                slot.Item.Use(slot, blockPosition);
            else
                Minicraft.World.GetBlock(blockPosition).Interact(blockPosition);
        }

        public int? Add(Item item, int amount = 1)
        {
            if (amount <= 0)
                throw new System.Exception("Amount must be greater than 0");
            if (item == Items.Nothing)
                throw new System.Exception("Cannot add nothing to inventory");
            // add to slots of same type
            var slots = GetSlotsOf(item);
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
                amountRemaining = slot.Set(item, amountRemaining.Value);
                if (!amountRemaining.HasValue)
                    return null;
            }
            return amountRemaining;
        }

        public void Draw()
        {
            // draw bar background
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (HotbarSize.X / 2f), Display.WindowSize.Y - HotbarSize.Y);
            Display.Draw(drawPos, HotbarSize.ToVector2(), new(color: Colors.HotbarBackground));
            // draw each slot background
            drawPos += new Vector2(Util.UI_SPACER);
            var drawSize = new Vector2(SLOT_SIZE);
            var slotOffset = new Vector2(Util.UI_SPACER + SLOT_SIZE, 0);
            Slot slot;
            // draw each slot
            for (var i = 0; i < _inventory.Length; i++)
            {
                slot = _inventory[i];
                // if active slot, draw selected border 1 px thick around slot
                if (i == _activeSlot)
                    Display.Draw(drawPos - new Vector2(2), drawSize + new Vector2(4), new(color: Colors.HotbarSelected));
                // draw slot background
                Display.Draw(drawPos, drawSize, new(color: Colors.HotbarSlotBackground));
                // draw item
                if (!slot.IsEmpty)
                {
                    // draw item
                    Display.Draw(drawPos, drawSize, slot.Item.DrawData);
                    // draw amount
                    if (!slot.IsEmpty)
                        Display.DrawStringWithShadow(FontSize._12, drawPos + new Vector2(Util.UI_SPACER, Util.UI_SPACER), slot.Amount.ToString(), Colors.HotbarSlotText);
                }
                // move draw position to next slot
                drawPos += slotOffset;
            }
        }

        public Slot this[int i] => _inventory[i];
    }
}
