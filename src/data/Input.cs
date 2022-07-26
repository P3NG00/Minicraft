using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Game.Data
{
    public sealed class Input
    {
        private readonly KeyboardState[] _keyStates = new KeyboardState[2];
        private readonly MouseState[] _mouseStates = new MouseState[2];

        public void Update()
        {
            _keyStates[1] = _keyStates[0];
            _keyStates[0] = Keyboard.GetState();
            _mouseStates[1] = _mouseStates[0];
            _mouseStates[0] = Mouse.GetState();
        }

        public bool KeyFirstDown(Keys key) => _keyStates[0].IsKeyDown(key) && _keyStates[1].IsKeyUp(key);

        public bool KeyHeld(Keys key) => _keyStates[0].IsKeyDown(key);

        public bool ButtonLeftFirstDown() => _mouseStates[0].LeftButton == ButtonState.Pressed && _mouseStates[1].LeftButton == ButtonState.Released;

        public bool ButtonRightFirstDown() => _mouseStates[0].RightButton == ButtonState.Pressed && _mouseStates[1].RightButton == ButtonState.Released;

        public Point MousePosition => _mouseStates[0].Position;

        public int ScrollWheel
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
