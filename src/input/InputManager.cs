using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Minicraft.Input
{
    internal static class InputManager
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

        internal static bool KeyPressedThisFrame(Keys key) => _keyStates[0].IsKeyDown(key) && _keyStates[1].IsKeyUp(key);

        internal static bool KeyReleasedThisFrame(Keys key) => _keyStates[0].IsKeyUp(key) && _keyStates[1].IsKeyDown(key);

        internal static bool KeyHeld(Keys key) => _keyStates[0].IsKeyDown(key);

        internal static bool MouseKeyPressedThisFrame(MouseKeys mouseKey)
        {
            switch (mouseKey)
            {
                case MouseKeys.Left: return _mouseStates[0].LeftButton == ButtonState.Pressed && _mouseStates[1].LeftButton == ButtonState.Released;
                case MouseKeys.Middle: return _mouseStates[0].MiddleButton == ButtonState.Pressed && _mouseStates[1].MiddleButton == ButtonState.Released;
                case MouseKeys.Right: return _mouseStates[0].RightButton == ButtonState.Pressed && _mouseStates[1].RightButton == ButtonState.Released;
                default: throw new System.NotImplementedException($"{nameof(MouseKeyPressedThisFrame)} not implemented for {mouseKey}");
            }
        }

        internal static bool MouseKeyReleasedThisFrame(MouseKeys mouseKey)
        {
            switch (mouseKey)
            {
                case MouseKeys.Left: return _mouseStates[0].LeftButton == ButtonState.Released && _mouseStates[1].LeftButton == ButtonState.Pressed;
                case MouseKeys.Middle: return _mouseStates[0].MiddleButton == ButtonState.Released && _mouseStates[1].MiddleButton == ButtonState.Pressed;
                case MouseKeys.Right: return _mouseStates[0].RightButton == ButtonState.Released && _mouseStates[1].RightButton == ButtonState.Pressed;
                default: throw new System.NotImplementedException($"{nameof(MouseKeyReleasedThisFrame)} not implemented for {mouseKey}");
            }
        }

        internal static bool MouseKeyHeld(MouseKeys mouseKey)
        {
            switch (mouseKey)
            {
                case MouseKeys.Left: return _mouseStates[0].LeftButton == ButtonState.Pressed;
                case MouseKeys.Middle: return _mouseStates[0].MiddleButton == ButtonState.Pressed;
                case MouseKeys.Right: return _mouseStates[0].RightButton == ButtonState.Pressed;
                default: throw new System.NotImplementedException($"{nameof(MouseKeyHeld)} not implemented for {mouseKey}");
            }
        }

        public static Point MousePosition => _mouseStates[0].Position;

        public static int ScrollWheelDelta
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
