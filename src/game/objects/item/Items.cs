using Microsoft.Xna.Framework;
using MinicraftGame.Texture;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Objects.ItemObject
{
    public sealed class Items : ObjectManager<Item>
    {
        private static Items _instance;

        public static Item Nothing { get; private set; }
        public static Item DebugStick { get; private set; }

        public Items() => Util.SingletonCheck(ref _instance, this);

        protected sealed override void InstantiateObjects()
        {
            Nothing = new Item("Nothing", new(), 0);
            DebugStick = new Item("Debug Stick", new(Textures.Stick, new Color(75, 55, 28)), 1);
            // TODO more items
        }

        protected sealed override void AddObjects(out Item[] items)
        {
            items = new[]
            {
                Nothing,    // 0
                DebugStick, // 1
            };
        }

        public static int Amount => _instance.ObjectAmount;

        public static Item FromID(int i) => _instance.ObjectFromID(i);
    }
}
