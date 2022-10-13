using System;
using Microsoft.Xna.Framework;
using Minicraft.Texture;

namespace Minicraft.Game.ItemType
{
    public static class Items
    {
        public static Item Nothing { get; private set; }
        public static Item DebugStick { get; private set; }

        private static Item[] _items;

        public static void Initialize()
        {
            Nothing = new Item("Nothing", new());
            DebugStick = new Item("Debug Stick", new(Textures.Stick, new Color(75, 55, 28)));
            // TODO

            _items = new[]
            {
                Nothing,
                DebugStick,
            };
        }

        public static int Amount => _items.Length;

        public static Item GetByID(int i) => _items[i];

        public static int GetID(Item item) => Array.IndexOf(_items, item);
    }
}
