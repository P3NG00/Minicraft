using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Font;
using MinicraftGame.Utils;

namespace MinicraftGame.UI
{
    public sealed class Button : AbstractHighlightable, ISceneObject
    {
        private readonly ColorTheme _colorTheme;
        private readonly string _text;
        private readonly Action _action;

        protected sealed override Rectangle GetRectangle
        {
            get
            {
                var pos = ((Display.WindowSize.ToVector2() * RelativeCenter) - (Size.ToVector2() / 2f)).ToPoint();
                return new Rectangle(pos, Size);
            }
        }

        // (0f, 0f) = top-left of window.
        // (1f, 1f) = bottom-right of window.
        public Button(Vector2 relativeCenter, Point size, string text, ColorTheme colorTheme, Action action) : base(relativeCenter, size)
        {
            _text = text;
            _colorTheme = colorTheme;
            _action = action;
        }

        public sealed override void Update()
        {
            // base call
            base.Update();
            // check if mouse was released over button
            if (Clicked)
                _action();
        }

        public void Draw()
        {
            // draw box
            Display.Draw(LastRectangle.Location.ToVector2(), Size.ToVector2(), new(color: Highlighted ? _colorTheme.MainHighlight : _colorTheme.Main));
            // draw text centered in box
            var color = Highlighted ? _colorTheme.TextHighlight : _colorTheme.Text;
            Display.DrawCenteredString(FontSize._12, RelativeCenter, _text, color, drawStringFunc: Display.DrawStringWithShadow);
        }
    }
}
