using System;
using Microsoft.Xna.Framework;
using Minicraft.Utils;

namespace Minicraft.UI
{
    public sealed class Button
    {
        // (0f, 0f) = top-left of window. (0.5f, 0.5f) = center of window. (1f, 1f) = bottom-right of window.
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
            _lastRect = new Rectangle(((Display.WindowSize.ToVector2() * _relativeCenter) - (_size.ToVector2() / 2f)).ToPoint(), _size);
            // test bounds
            _highlighted = _lastRect.Contains(Input.MousePosition);
            if (Input.MouseLeftFirstUp() && _highlighted)
                Action();
        }

        public void Draw()
        {
            // draw box
            Display.Draw(_lastRect, _highlighted && ColorBoxHighlight.HasValue ? ColorBoxHighlight.Value : ColorBox);
            // draw text centered in box
            var drawPos = _lastRect.Center.ToVector2() - (Display.TypeWriter_12.MeasureString(_text) / 2f);
            Display.DrawString(Display.TypeWriter_12, drawPos, _text, _highlighted && ColorTextHighlight.HasValue ? ColorTextHighlight.Value : ColorText);
        }
    }
}
