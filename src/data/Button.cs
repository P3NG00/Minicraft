using System;
using Microsoft.Xna.Framework;

namespace Game.Data
{
    public sealed class Button
    {
        // (0f, 0f) = top-left of window. (0.5f, 0.5f) = center of window. (1f, 1f) = bottom-right of window.
        private readonly Vector2 _relativeCenter;
        private readonly Point _size;
        private readonly string _text;
        private readonly Color _color;
        private readonly Action _action;

        private Rectangle _lastRect;

        public Button(Vector2 relativeCenter, Point size, string text, Color color, Action actionOnClick)
        {
            _relativeCenter = relativeCenter;
            _size = size;
            _text = text;
            _color = color;
            _action = actionOnClick;
        }

        private Rectangle Rectangle
        {
            get
            {
                var pos = ((Display.WindowSize.ToVector2() * _relativeCenter) - (_size.ToVector2() / 2f)).ToPoint();
                return new Rectangle(pos, _size);
            }
        }

        public void Update()
        {
            // test if clicked
            _lastRect = Rectangle;
            if (Input.MouseLeftFirstUp() && _lastRect.Contains(Input.MousePosition))
                _action();
        }

        public void Draw()
        {
            // draw box
            Display.Draw(_lastRect, _color);
            // TODO draw text centered in box
            var textSize = Display.FontUI.MeasureString(_text);
            var drawPos = _lastRect.Center.ToVector2() - (textSize / 2f);
            Display.DrawString(Display.FontUI, drawPos, _text, Colors.UI_TextMenu);
        }
    }
}
