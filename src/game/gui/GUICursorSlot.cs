using MinicraftGame.Game.Inventories;
using MinicraftGame.Input;

namespace MinicraftGame.Game.GUI
{
    public sealed class GUICursorSlot
    {
        private Slot _slot = new();

        public void Swap(ref Slot slot)
        {
            var swap = slot;
            slot = _slot;
            _slot = swap;
        }

        public void Draw() => _slot.Draw(InputManager.MousePosition.ToVector2());
    }
}
