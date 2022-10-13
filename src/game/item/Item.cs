using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Game.Entities.Living;
using Minicraft.Game.Inventories;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.ItemType
{
    public class Item
    {
        public readonly string Name;
        public readonly DrawData DrawData;

        public Texture2D Texture => DrawData.Texture;
        public Color Color => DrawData.Color;

        public Item(string name, DrawData drawData)
        {
            Name = name;
            DrawData = drawData;
        }

        public virtual void Use(World world, Slot slot, PlayerEntity player, Point blockPosition) {}
    }
}
