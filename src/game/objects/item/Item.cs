using Microsoft.Xna.Framework;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects.ItemObject
{
    public class Item : GameObject
    {
        public Item(string name, DrawData drawData, int id) : base (name, drawData, id) {}

        // TODO override in future for items with special purposes
        public virtual void Use(Slot slot, Point blockPosition) => Minicraft.World.Interact(blockPosition);
    }
}
