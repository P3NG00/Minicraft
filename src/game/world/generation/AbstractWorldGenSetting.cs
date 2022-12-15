using System;
using Microsoft.Xna.Framework;
using MinicraftGame.Input;
using MinicraftGame.UI;
using MinicraftGame.Utils;

namespace MinicraftGame.Game.Worlds.Generation
{
    public abstract class AbstractWorldGenSetting<T> : AbstractHighlightable, IWorldGenSetting where T : IComparable<T>
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

        private readonly Button _buttonDecrement;
        private readonly Button _buttonIncrement;

        public T Value;

        protected T StepValue => Keybinds.Shift.Held ? StepShift : Step;
        protected sealed override Rectangle GetRectangle
        {
            get
            {
                var pos = ((Display.WindowSize.ToVector2() * RelativeCenter) - (Size.ToVector2() / 2f)).ToPoint();
                return new Rectangle(pos, Size);
            }
        }

        private Action _onIncrement = null;
        private Action _onDecrement = null;

        public AbstractWorldGenSetting(Vector2 relativeCenter, string name, T defaultValue, T min, T max, T step, T stepShift) : base(relativeCenter)
        {
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

        protected sealed override Point Size
        {
            get
            {
                var windowSize = Display.WindowSize.ToVector2();
                var width = (int)(((RelativeCenter.X + RELATIVE_BUTTON_OFFSET) - (RelativeCenter.X - RELATIVE_BUTTON_OFFSET)) * windowSize.X) + BUTTON_SIZE;
                return new Point(width, BUTTON_SIZE);
            }
        }

        public void Increment()
        {
            // increment value if below max
            if (Value.CompareTo(Max) < 0)
            {
                IncrementFunc();
                // ensure value at most max
                if (Value.CompareTo(Max) > 0)
                    Value = Max;
                _onIncrement?.Invoke();
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
                _onDecrement?.Invoke();
            }
        }

        public void SetOnIncrement(Action onIncrement) => _onIncrement = onIncrement;

        public void SetOnDecrement(Action onDecrement) => _onDecrement = onDecrement;

        public sealed override void Update()
        {
            // base call
            base.Update();
            // check scroll wheel
            if (Highlighted)
            {
                var scroll = InputManager.ScrollWheelDelta;
                if (scroll > 0)
                    Increment();
                else if (scroll < 0)
                    Decrement();
            }
            // update buttons
            _buttonDecrement.Update();
            _buttonIncrement.Update();
        }

        public void Draw()
        {
            // draw highlight
            if (Highlighted)
                Display.Draw(LastRectangle.Location.ToVector2(), Size.ToVector2(), new(color: Colors.SettingHighlight));
            // draw buttons
            _buttonDecrement.Draw();
            _buttonIncrement.Draw();
            // draw name label
            var textOffset = new Vector2(0, RELATIVE_TEXT_OFFSET);
            Display.DrawCenteredString(Font.FontSize._12, RelativeCenter - textOffset, Name, Colors.TextWorldGenSetting);
            // draw value label
            Display.DrawCenteredString(Font.FontSize._12, RelativeCenter + textOffset, Value.ToString(), Colors.TextWorldGenSetting);
        }

        public static implicit operator T(AbstractWorldGenSetting<T> setting) => setting.Value;
    }
}
