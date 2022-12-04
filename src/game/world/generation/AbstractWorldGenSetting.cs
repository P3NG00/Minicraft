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

        private readonly Vector2 _relativeScreenPosition;
        private readonly Button _buttonDecrement;
        private readonly Button _buttonIncrement;

        public T Value;

        protected T StepValue => Keybinds.Shift.Held ? StepShift : Step;

        // TODO '_highlighted' to display a highlight behind this setting box when drawing and hovered over with mouse
        // TODO when highlighted, allow scrollwheel up to increment and down to decrement

        public AbstractWorldGenSetting(Vector2 relativeScreenPosition, string name, T defaultValue, T min, T max, T step, T stepShift)
        {
            _relativeScreenPosition = relativeScreenPosition;
            var buttonOffset = new Vector2(RELATIVE_BUTTON_OFFSET, 0);
            var buttonSize = new Point(BUTTON_SIZE);
            _buttonDecrement = new(relativeScreenPosition - buttonOffset, buttonSize, TEXT_DECREMENT, Colors.ThemeDefault, Decrement);
            _buttonIncrement = new(relativeScreenPosition + buttonOffset, buttonSize, TEXT_INCREMENT, Colors.ThemeDefault, Increment);
            Name = name;
            Value = defaultValue;
            Min = min;
            Max = max;
            Step = step;
            StepShift = stepShift;
        }

        public abstract void IncrementFunc();

        public abstract void DecrementFunc();

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
            // update buttons
            _buttonDecrement.Update();
            _buttonIncrement.Update();
            // TODO add ability to change value with scrollwheel while hovering over area of entire setting (use _lastRect like you did in UI.Button class)
        }

        public void Draw()
        {
            // draw buttons
            _buttonDecrement.Draw();
            _buttonIncrement.Draw();
            // draw name label
            var textOffset = new Vector2(0, RELATIVE_TEXT_OFFSET);
            Display.DrawCenteredString(Font.FontSize._12, _relativeScreenPosition - textOffset, Name, Colors.TextWorldGenSetting);
            // draw value label
            Display.DrawCenteredString(Font.FontSize._12, _relativeScreenPosition + textOffset, Value.ToString(), Colors.TextWorldGenSetting);
        }

        public static implicit operator T(AbstractWorldGenSetting<T> setting) => setting.Value;
    }
}
