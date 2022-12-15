using Microsoft.Xna.Framework;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.GUI
{
    public sealed class GUIHotbar
    {
        public const int HOTBAR_WIDTH = (GUIItemSlot.SIZE * Inventory.SLOTS_WIDTH) + (Util.UI_SPACER * (Inventory.SLOTS_WIDTH + 1));
        public const int HOTBAR_HEIGHT = GUIItemSlot.SIZE + (Util.UI_SPACER * 2);

        public static Point HotbarSize => new(HOTBAR_WIDTH, HOTBAR_HEIGHT);

        private readonly GUIItemSlot[] _slots = new GUIItemSlot[Inventory.SLOTS_WIDTH];

        public int ActiveSlot = 0;

        public GUIHotbar()
        {
            // relative y offset of gui item slots
            const int Y = -((GUIItemSlot.SIZE / 2) + Util.UI_SPACER);
            const int HALF_X = Inventory.SLOTS_WIDTH / 2;
            for (int i = 0; i < Inventory.SLOTS_WIDTH; i++)
            {
                var j = i - HALF_X;
                var x = (j * GUIItemSlot.SIZE) + (j * Util.UI_SPACER);
                // relative x offset of gui item slots
                _slots[i] = new GUIItemSlot(new(x, Y), i);
            }
        }

        public void Update()
        {
            var screenPos = Display.WindowSize.ToVector2() * new Vector2(0.5f, 1f);
            foreach (var slot in _slots)
            {
                slot.SetScreenPos(screenPos);
                slot.Update();
            }
        }

        public void Draw()
        {
            // draw bar background
            var drawPos = new Vector2((Display.WindowSize.X / 2f) - (HOTBAR_WIDTH / 2f), Display.WindowSize.Y - HOTBAR_HEIGHT);
            Display.Draw(drawPos, HotbarSize.ToVector2(), new(color: Colors.HotbarBackground));
            // draw slots
            for (int i = 0; i < Inventory.SLOTS_WIDTH; i++)
            {
                var slot = _slots[i];
                var isSelected = i == ActiveSlot;
                slot.Draw(isSelected);
            }
        }
    }
}
