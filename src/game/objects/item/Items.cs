using Microsoft.Xna.Framework;
using MinicraftGame.Texture;

namespace MinicraftGame.Game.Objects.ItemObject
{
    public sealed class Items : ObjectManager<Item>
    {
        private static ObjectManager<Item> _instance;

        public static Item Nothing { get; private set; }
        public static Item DebugStick { get; private set; }

        protected sealed override Item[] ObjectArray => new[]
        {
            Nothing,    // 0
            DebugStick, // 1
        };

        public Items() : base(ref _instance) {}

        protected sealed override void InstantiateObjects()
        {
            Nothing = new Item("Nothing", new(), 0);
            DebugStick = new Item("Debug Stick", new(Textures.Stick, new Color(75, 55, 28)), 1);
            // TODO more items
        }

        public static int Amount => _instance.ObjectAmount;

        public static Item FromID(int i) => _instance.ObjectFromID(i);
    }
}
