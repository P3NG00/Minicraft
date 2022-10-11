using Microsoft.Xna.Framework.Input;

namespace Minicraft.Utils
{
    public static class Keybinds
    {
        public static readonly MouseKeyInput MouseLeft = new MouseKeyInput(MouseKeys.Left);
        public static readonly MouseKeyInput MouseMiddle = new MouseKeyInput(MouseKeys.Middle);
        public static readonly MouseKeyInput MouseRight = new MouseKeyInput(MouseKeys.Right);

        public static readonly KeyInput Pause = new(Keys.Escape);
        public static readonly KeyInput DebugCheckUpdates = new(Keys.F9);
        public static readonly KeyInput Debug = new(Keys.F12);
        public static readonly KeyInput Fullscreen = new(Keys.F11);
        public static readonly KeyInput MoveLeft = new(Keys.A);
        public static readonly KeyInput MoveRight = new(Keys.D);
        public static readonly KeyInput Jump = new(Keys.Space);
        public static readonly KeyInput Run = new(Keys.LeftShift);
        public static readonly KeyInput Shift = new(Keys.LeftShift);
        public static readonly KeyInput SpawnProjectile = new(Keys.Q);
        public static readonly KeyInput SpawnBouncyProjectile = new(Keys.E);
        public static readonly KeyInput ToggleGridMode = new(Keys.Tab);

        public static readonly KeyInput TimeScaleDecrement = new(Keys.F1);
        public static readonly KeyInput TimeScaleIncrement = new(Keys.F2);
        public static readonly KeyInput TimeTickStep = new(Keys.F3);
    }
}
