using Microsoft.Xna.Framework.Input;

namespace Minicraft.Input
{
    public sealed class KeyInput : AbstractInput
    {
        private readonly Keys _key;

        public KeyInput(Keys key) => _key = key;

        public sealed override bool PressedThisFrame => InputManager.KeyPressedThisFrame(_key);

        public sealed override bool ReleasedThisFrame => InputManager.KeyReleasedThisFrame(_key);

        public sealed override bool Held => InputManager.KeyHeld(_key);
    }
}
