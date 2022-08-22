using System;
using Microsoft.Xna.Framework;
using Minicraft.Font;
using Minicraft.Utils;

namespace Minicraft.UI
{
    public sealed class Button
    {
        private readonly Vector2 _relativeCenter;
        private readonly Point _size;
        private readonly string _text;

        public Action Action;
        public Color ColorBox;
        public Color ColorText;
        public Color? ColorBoxHighlight = null;
        public Color? ColorTextHighlight = null;

        private Rectangle _lastRect;
        private bool _highlighted = false;

        // ( 0f,  0f) = top-left of window.
        // ( 1f,  1f) = bottom-right of window.
        public Button(Vector2 relativeCenter, Point size, string text, Color colorBox, Color colorText)
        {
            _relativeCenter = relativeCenter;
            _size = size;
            _text = text;
            ColorBox = colorBox;
            ColorText = colorText;
        }

        public void Update()
        {
            // find rectangle bounds
            var pos = ((Display.WindowSize.ToVector2() * _relativeCenter) - (_size.ToVector2() / 2f)).ToPoint();
            _lastRect = new Rectangle(pos, _size);
            // test bounds
            _highlighted = _lastRect.Contains(Input.MousePosition);
            if (Input.MouseLeftFirstUp() && _highlighted)
                Action();
        }

        public void Draw()
        {
            // draw box
            Display.Draw(_lastRect.Location.ToVector2(), _lastRect.Size.ToVector2(), _highlighted && ColorBoxHighlight.HasValue ? ColorBoxHighlight.Value : ColorBox);
            // draw text centered in box
            var color = _highlighted && ColorTextHighlight.HasValue ? ColorTextHighlight.Value : ColorText;
            Display.DrawCenteredText(FontSize._12, _relativeCenter, _text, color, Display.DrawStringWithShadow);
        }
    }
}
