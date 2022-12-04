using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Input;
using MinicraftGame.UI;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Worlds.Generation
{
    public abstract class AbstractWorldGenSetting<T> : IWorldGenSetting where T : IComparable<T>
    {
        private const string TEXT_DECREMENT = "<";
        private const string TEXT_INCREMENT = ">";
        private const float RELATIVE_BUTTON_OFFSET = 0.15f;
        private const float RELATIVE_TEXT_OFFSET = 0.015f;
        private const int BUTTON_SIZE = 50;

        public readonly string Name;

        protected readonly T Min;
        protected readonly T Max;
        protected readonly T Step;
        protected readonly T StepShift;

        private readonly Vector2 _relativeCenter;
        private readonly Button _buttonDecrement;
        private readonly Button _buttonIncrement;

        public T Value;

        protected T StepValue => Keybinds.Shift.Held ? StepShift : Step;

        private Rectangle _lastRect;
        private bool _highlighted;

        public AbstractWorldGenSetting(Vector2 relativeCenter, string name, T defaultValue, T min, T max, T step, T stepShift)
        {
            _relativeCenter = relativeCenter;
            var buttonOffset = new Vector2(RELATIVE_BUTTON_OFFSET, 0);
            var buttonSize = new Point(BUTTON_SIZE);
            _buttonDecrement = new(relativeCenter - buttonOffset, buttonSize, TEXT_DECREMENT, Colors.ThemeDefault, Decrement);
            _buttonIncrement = new(relativeCenter + buttonOffset, buttonSize, TEXT_INCREMENT, Colors.ThemeDefault, Increment);
            Name = name;
            Value = defaultValue;
            Min = min;
            Max = max;
            Step = step;
            StepShift = stepShift;
        }

        protected abstract void IncrementFunc();

        protected abstract void DecrementFunc();

        public void Increment()
        {
            // increment value if below max
            if (Value.CompareTo(Max) < 0)
            {
                IncrementFunc();
                // ensure value at most max
                if (Value.CompareTo(Max) > 0)
                    Value = Max;
            }
        }

        public void Decrement()
        {
            // decrement value if above min
            if (Value.CompareTo(Min) > 0)
            {
                DecrementFunc();
                // ensure value at least min
                if (Value.CompareTo(Min) < 0)
                    Value = Min;
            }
        }

        public void Update()
        {
            // find rectangle bounds
            var windowSize = Display.WindowSize.ToVector2();
            var width = (int)(((_relativeCenter.X + RELATIVE_BUTTON_OFFSET) - (_relativeCenter.X - RELATIVE_BUTTON_OFFSET)) * windowSize.X) + BUTTON_SIZE;
            var size = new Point(width, BUTTON_SIZE);
            var pos = ((windowSize * _relativeCenter) - (size.ToVector2() / 2f)).ToPoint();
            _lastRect = new Rectangle(pos, size);
            // update highlighted
            _highlighted = _lastRect.Contains(InputManager.MousePosition);
            // check scroll wheel
            if (_highlighted)
            {
                if (InputManager.ScrollWheelDelta > 0)
                    Increment();
                else if (InputManager.ScrollWheelDelta < 0)
                    Decrement();
            }
            // update buttons
            _buttonDecrement.Update();
            _buttonIncrement.Update();
        }

        public void Draw()
        {
            // draw highlight
            if (_highlighted)
                Display.Draw(_lastRect.Location.ToVector2(), _lastRect.Size.ToVector2(), new(color: Colors.SettingHighlight));
            // draw buttons
            _buttonDecrement.Draw();
            _buttonIncrement.Draw();
            // draw name label
            var textOffset = new Vector2(0, RELATIVE_TEXT_OFFSET);
            Display.DrawCenteredString(Font.FontSize._12, _relativeCenter - textOffset, Name, Colors.TextWorldGenSetting);
            // draw value label
            Display.DrawCenteredString(Font.FontSize._12, _relativeCenter + textOffset, Value.ToString(), Colors.TextWorldGenSetting);
        }

        public static implicit operator T(AbstractWorldGenSetting<T> setting) => setting.Value;
    }
}
