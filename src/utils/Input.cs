using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Minicraft.Utils
{
    public static class Input
    {
        private static readonly KeyboardState[] _keyStates = new KeyboardState[2];
        private static readonly MouseState[] _mouseStates = new MouseState[2];

        public static void Update()
        {
            _keyStates[1] = _keyStates[0];
            _keyStates[0] = Keyboard.GetState();
            _mouseStates[1] = _mouseStates[0];
            _mouseStates[0] = Mouse.GetState();
        }

        public static bool KeyFirstDown(Keys key) => _keyStates[0].IsKeyDown(key) && _keyStates[1].IsKeyUp(key);

        public static bool KeyHeld(Keys key) => _keyStates[0].IsKeyDown(key);

        public static bool MouseLeftHeld() => _mouseStates[0].LeftButton == ButtonState.Pressed;

        public static bool MouseMiddleHeld() => _mouseStates[0].MiddleButton == ButtonState.Pressed;

        public static bool MouseRightHeld() => _mouseStates[0].RightButton == ButtonState.Pressed;

        public static bool MouseLeftFirstDown() => _mouseStates[0].LeftButton == ButtonState.Pressed && _mouseStates[1].LeftButton == ButtonState.Released;

        public static bool MouseMiddleFirstDown() => _mouseStates[0].MiddleButton == ButtonState.Pressed && _mouseStates[1].MiddleButton == ButtonState.Released;

        public static bool MouseRightFirstDown() => _mouseStates[0].RightButton == ButtonState.Pressed && _mouseStates[1].RightButton == ButtonState.Released;

        public static bool MouseLeftFirstUp() => _mouseStates[0].LeftButton == ButtonState.Released && _mouseStates[1].LeftButton == ButtonState.Pressed;

        public static Point MousePosition => _mouseStates[0].Position;

        public static int ScrollWheel
        {
            get
            {
                if (_mouseStates[0].ScrollWheelValue > _mouseStates[1].ScrollWheelValue)
                    return 1;
                else if (_mouseStates[0].ScrollWheelValue < _mouseStates[1].ScrollWheelValue)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
