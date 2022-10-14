namespace MinicraftGame.Input
{
    public sealed class MouseKeyInput : AbstractInput<MouseKeys>
    {
        public MouseKeyInput(MouseKeys mouseKey) : base(mouseKey) {}

        public sealed override bool PressedThisFrame => InputManager.MouseKeyPressedThisFrame(InputType);

        public sealed override bool ReleasedThisFrame => InputManager.MouseKeyReleasedThisFrame(InputType);

        public sealed override bool Held => InputManager.MouseKeyHeld(InputType);
    }
}
