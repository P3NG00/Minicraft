namespace MinicraftGame.Input
{
    public sealed class MouseInput : AbstractInput<Mouse>
    {
        public MouseInput(Mouse mouseKey) : base(mouseKey) {}

        public sealed override bool PressedThisFrame => InputManager.MousePressedThisFrame(InputType);

        public sealed override bool ReleasedThisFrame => InputManager.MouseReleasedThisFrame(InputType);

        public sealed override bool Held => InputManager.MouseHeld(InputType);
    }
}
