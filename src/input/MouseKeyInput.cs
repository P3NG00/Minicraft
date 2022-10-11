namespace Minicraft.Input
{
    public sealed class MouseKeyInput : AbstractInput
    {
        private readonly MouseKeys _mouseKey;

        public MouseKeyInput(MouseKeys mouseKey) => _mouseKey = mouseKey;

        public sealed override bool PressedThisFrame => InputManager.MouseKeyPressedThisFrame(_mouseKey);

        public sealed override bool ReleasedThisFrame => InputManager.MouseKeyReleasedThisFrame(_mouseKey);

        public sealed override bool Held => InputManager.MouseKeyHeld(_mouseKey);
    }
}
