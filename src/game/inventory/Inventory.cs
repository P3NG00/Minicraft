using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.Objects.ItemObject;

namespace MinicraftGame.Game.Inventories
{
    public sealed class Inventory
    {
        public const int SLOTS_WIDTH = 9;
        public const int SLOTS_HEIGHT = 4;
        public const int SLOTS = SLOTS_WIDTH * SLOTS_HEIGHT;

        private readonly Slot[] _inventory = new Slot[SLOTS];

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

        public void Use(int slotId, Point blockPosition)
        {
            var slot = _inventory[slotId];
            if (!slot.IsEmpty)
                slot.Item.Use(slot, blockPosition);
            else
                Minicraft.World.Interact(blockPosition);
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

        public ref Slot this[int i] => ref _inventory[i];
    }
}
