namespace Minicraft.Utils
{
    public sealed class MouseKeyInput : AbstractInput
    {
        private readonly MouseKeys _mouseKey;

        public MouseKeyInput(MouseKeys mouseKey) => _mouseKey = mouseKey;

        public sealed override bool PressedThisFrame => Input.MouseKeyPressedThisFrame(_mouseKey);

        public sealed override bool ReleasedThisFrame => Input.MouseKeyReleasedThisFrame(_mouseKey);

        public sealed override bool Held => Input.MouseKeyHeld(_mouseKey);
    }
}
