using Microsoft.Xna.Framework.Input;

namespace Minicraft.Utils
{
    public sealed class KeyInput : AbstractInput
    {
        private readonly Keys _key;

        public KeyInput(Keys key) => _key = key;

        public sealed override bool PressedThisFrame => Input.KeyPressedThisFrame(_key);

        public sealed override bool ReleasedThisFrame => Input.KeyReleasedThisFrame(_key);

        public sealed override bool Held => Input.KeyHeld(_key);
    }
}
