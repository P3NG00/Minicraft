using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MinicraftGame.Input
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

        internal static bool MousePressedThisFrame(MouseKeys mouseKey)
        {
            var states = GetMouseButtonStates(mouseKey);
            return states.previous == ButtonState.Released && states.current == ButtonState.Pressed;
        }

        internal static bool MouseReleasedThisFrame(MouseKeys mouseKey)
        {
            var states = GetMouseButtonStates(mouseKey);
            return states.previous == ButtonState.Pressed && states.current == ButtonState.Released;
        }

        internal static bool MouseHeld(MouseKeys mouseKey)
        {
            var states = GetMouseButtonStates(mouseKey);
            return states.current == ButtonState.Pressed;
        }

        private static (ButtonState previous, ButtonState current) GetMouseButtonStates(MouseKeys mouseKey)
        {
            switch (mouseKey)
            {
                case MouseKeys.Left: return (_mouseStates[1].LeftButton, _mouseStates[0].LeftButton);
                case MouseKeys.Middle: return (_mouseStates[1].MiddleButton, _mouseStates[0].MiddleButton);
                case MouseKeys.Right: return (_mouseStates[1].RightButton, _mouseStates[0].RightButton);
                default: throw new System.NotImplementedException($"{nameof(GetMouseButtonStates)} not implemented for {mouseKey}");
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
