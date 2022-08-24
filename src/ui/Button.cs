using System;
using Microsoft.Xna.Framework;
using Minicraft.Font;
using Minicraft.Utils;

namespace Minicraft.UI
{
    public sealed class Button
    {
        private readonly Vector2 _relativeCenter;
        private readonly ColorTheme _colorTheme;
        private readonly Point _size;
        private readonly string _text;
        private Action _action;

        private Rectangle _lastRect;
        private bool _highlighted = false;

        // (0f, 0f) = top-left of window.
        // (1f, 1f) = bottom-right of window.
        public Button(Vector2 relativeCenter, Point size, string text, ColorTheme colorTheme, Action action = null)
        {
            _relativeCenter = relativeCenter;
            _size = size;
            _text = text;
            _colorTheme = colorTheme;
            _action = action;
        }

        public void SetAction(Action action) => _action = action;

        public void Update()
        {
            // find rectangle bounds
            var pos = ((Display.WindowSize.ToVector2() * _relativeCenter) - (_size.ToVector2() / 2f)).ToPoint();
            _lastRect = new Rectangle(pos, _size);
            // test bounds
            _highlighted = _lastRect.Contains(Input.MousePosition);
            if (Input.MouseLeftFirstUp() && _highlighted)
                _action();
        }

        public void Draw()
        {
            // draw box
            Display.Draw(_lastRect.Location.ToVector2(), _lastRect.Size.ToVector2(), _highlighted ? _colorTheme.MainHighlight : _colorTheme.Main);
            // draw text centered in box
            var color = _highlighted ? _colorTheme.TextHighlight : _colorTheme.Text;
            Display.DrawCenteredString(FontSize._12, _relativeCenter, _text, color, Display.DrawStringWithShadow);
        }
    }
}
