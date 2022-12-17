using Microsoft.Xna.Framework;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.GUI
{
    public sealed class GUIInventory
    {
        private GUIItemSlot[] _slots = new GUIItemSlot[Inventory.SLOTS_WIDTH * (Inventory.SLOTS_HEIGHT - 1)];

        public GUIInventory()
        {
            const int HALF_X = Inventory.SLOTS_WIDTH / 2;
            const int HALF_Y = (Inventory.SLOTS_HEIGHT - 1) / 2;
            for (int y = 0; y < Inventory.SLOTS_HEIGHT - 1; y++)
            {
                for (int x = 0; x < Inventory.SLOTS_WIDTH; x++)
                {
                    var i = x - HALF_X;
                    var j = y - HALF_Y;
                    var posX = (i * GUIItemSlot.SIZE) + (i * Util.UI_SPACER);
                    var posY = (j * GUIItemSlot.SIZE) + (j * Util.UI_SPACER);
                    var slotId = ((y + 1) * Inventory.SLOTS_WIDTH) + x;
                    _slots[(y * Inventory.SLOTS_WIDTH) + x] = new GUIItemSlot(new(posX, posY), slotId);
                }
            }
        }

        public void Update()
        {
            var screenPos = Display.WindowSize.ToVector2() * new Vector2(0.5f, 0.75f);
            foreach (var slot in _slots)
            {
                slot.SetScreenPos(screenPos);
                slot.Update();
            }
        }

        public void Draw() => _slots.ForEach(slot => slot.Draw(false));
    }
}
