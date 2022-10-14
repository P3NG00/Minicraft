using Microsoft.Xna.Framework;
using MinicraftGame.Texture;

namespace MinicraftGame.Game.ItemType
{
    public static class Items
    {
        public static Item Nothing { get; private set; }
        public static Item DebugStick { get; private set; }

        private static Item[] _items;

        public static void Initialize()
        {
            // instantiate items
            Nothing = new Item("Nothing", new(), 0);
            DebugStick = new Item("Debug Stick", new(Textures.Stick, new Color(75, 55, 28)), 1);
            // TODO more items
            // add to array in order of id
            _items = new[]
            {
                Nothing,    // 0
                DebugStick, // 1
            };
            // check item id's
            for (int i = 0; i < _items.Length; i++)
                if (_items[i].ID != i)
                    throw new System.Exception($"Item ID mismatch: {_items[i].Name} has ID {_items[i].ID} but is at index {i}.");
        }

        public static int Amount => _items.Length;

        public static Item FromID(int i) => _items[i];
    }
}
