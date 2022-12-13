using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects.ItemObject
{
    public class Item : GameObject, IEquatable<Item>
    {
        public Item(string name, DrawData drawData, int id) : base (name, drawData, id) {}

        // TODO override in future for items with special purposes
        public virtual void Use(Slot slot, Point blockPosition) => Minicraft.World.GetBlock(blockPosition).Interact(blockPosition);

        public bool Equals(Item other)
        {
            if (ReferenceEquals(other, null))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Name.Equals(other.Name) && DrawData.Equals(other.DrawData);
        }

        public sealed override bool Equals(object obj) => Equals(obj as Item);

        public sealed override int GetHashCode() => Name.GetHashCode() ^ DrawData.GetHashCode();

        public static bool operator ==(Item a, Item b)
        {
            bool aNull = ReferenceEquals(a, null);
            bool bNull = ReferenceEquals(b, null);
            if (aNull && bNull)
                return true;
            if (aNull || bNull)
                return false;
            if (ReferenceEquals(a, b))
                return true;
            return a.Equals(b);
        }

        public static bool operator !=(Item a, Item b) => !(a == b);
    }
}
