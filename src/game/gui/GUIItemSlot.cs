using Microsoft.Xna.Framework;
using MinicraftGame.Font;
using MinicraftGame.Scenes;
using MinicraftGame.Texture;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.GUI
{
    public sealed class GUIItemSlot : AbstractHighlightable
    {
        public const int SIZE = Textures.SIZE * 6;

        private readonly int _slotId;

        private Vector2 _screenPosition;

        protected override Rectangle GetRectangle
        {
            get
            {
                var drawPos = _screenPosition + RelativeCenter - new Vector2(SIZE / 2f);
                return new Rectangle(drawPos.ToPoint(), Size);
            }
        }

        public GUIItemSlot(Vector2 relativeOffset, int slotId) : base(relativeOffset, new(SIZE)) => _slotId = slotId;

        public void SetScreenPos(Vector2 screenPos) => _screenPosition = screenPos;

        public sealed override void Update()
        {
            // base call
            base.Update();
            // when clicked, swap with cursor slot
            if (Clicked)
            {
                var cursorSlot = GameScene.CursorSlot;
                var slot = Minicraft.Player.Inventory[_slotId];
                if (cursorSlot == null || cursorSlot.IsEmpty)
                {
                    // pick up item
                    GameScene.CursorSlot = slot;
                    Minicraft.Player.Inventory[_slotId] = new();
                }
                else if (slot.IsEmpty)
                {
                    // place item
                    Minicraft.Player.Inventory[_slotId] = cursorSlot;
                    GameScene.CursorSlot = new();
                }
                else
                {
                    // swap items
                    Minicraft.Player.Inventory[_slotId] = cursorSlot;
                    GameScene.CursorSlot = slot;
                }
            }
        }

        public void Draw(bool isSelected)
        {
            var drawPos = LastRectangle.Location.ToVector2();
            var drawSize = Size.ToVector2();
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
            // draw highlight
            if (Highlighted)
                Display.Draw(drawPos, drawSize, new(color: Colors.HotbarSlotHighlight));
        }
    }
}
