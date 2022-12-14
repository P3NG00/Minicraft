using Microsoft.Xna.Framework;
using MinicraftGame.Font;
using MinicraftGame.Texture;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.GUI
{
    public sealed class GUIItemSlot
    {
        public const int SIZE = Textures.SIZE * 6;

        private readonly Vector2 _relativeOffset;
        private readonly int _slotId;

        public GUIItemSlot(Vector2 relativeOffset, int slotId)
        {
            _relativeOffset = relativeOffset;
            _slotId = slotId;
        }

        public void Draw(Vector2 screenPosition, bool isSelected)
        {
            var drawPos = screenPosition + _relativeOffset - new Vector2(SIZE / 2f);
            var drawSize = new Vector2(GUIItemSlot.SIZE);
            // draw border around selected slot
            if (isSelected)
                Display.Draw(drawPos - new Vector2(2), drawSize + new Vector2(4), new(color: Colors.HotbarSelected));
            // draw slot background
            Display.Draw(drawPos, drawSize, new(color: Colors.HotbarSlotBackground));
            // draw item
            var slot = Minicraft.Player.Inventory[_slotId];
            if (!slot.IsEmpty)
            {
                // draw item
                Display.Draw(drawPos, drawSize, slot.Item.DrawData);
                // draw amount
                if (!slot.IsEmpty)
                    Display.DrawStringWithShadow(FontSize._12, drawPos + new Vector2(Util.UI_SPACER), slot.Amount.ToString(), Colors.HotbarSlotText);
            }
        }
    }
}
