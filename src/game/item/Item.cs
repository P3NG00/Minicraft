using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Minicraft.Game.Entities.Living;
using Minicraft.Game.Inventories;
using Minicraft.Game.Worlds;
using Minicraft.Utils;

namespace Minicraft.Game.ItemType
{
    public sealed class Item
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

        // TODO override for BlockItem
        public void Use(World world, Slot slot, PlayerEntity player, Vector2 mousePosition, Point blockPosition)
        {
            // TODO
        }
    }
}
