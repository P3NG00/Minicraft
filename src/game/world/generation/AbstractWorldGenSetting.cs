using Microsoft.Xna.Framework;
using Minicraft.UI;
using Minicraft.Utils;

namespace Minicraft.Game.Worlds.Generation
{
    public abstract class AbstractWorldGenSetting<T> : IWorldGenSetting
    {
        public readonly string Name;

        protected readonly T Min;
        protected readonly T Max;
        protected readonly T Step;
        protected readonly T StepShift;

        private readonly Vector2 _relativeScreenPosition;
        private readonly Button _buttonDecrement;
        private readonly Button _buttonIncrement;

        public T Value;

        protected bool IsShiftPressed => Input.KeyHeld(Keybinds.Shift);
        protected T StepValue => IsShiftPressed ? StepShift : Step;

        public AbstractWorldGenSetting(Vector2 relativeScreenPosition, string name, T defaultValue, T min, T max, T step, T stepShift)
        {
            _relativeScreenPosition = relativeScreenPosition;
            _buttonDecrement = new(relativeScreenPosition - new Vector2(0.15f, 0), new Point(50, 50), "<", Colors.ThemeDefault, Decrement);
            _buttonIncrement = new(relativeScreenPosition + new Vector2(0.15f, 0), new Point(50, 50), ">", Colors.ThemeDefault, Increment);
            Name = name;
            Value = defaultValue;
            Min = min;
            Max = max;
            Step = step;
            StepShift = stepShift;
        }

        public abstract void Increment();

        public abstract void Decrement();

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
            Display.DrawCenteredString(Font.FontSize._12, _relativeScreenPosition - new Vector2(0, 0.015f), Name, Colors.TextWorldGenSetting);
            // draw value label
            Display.DrawCenteredString(Font.FontSize._12, _relativeScreenPosition + new Vector2(0, 0.015f), Value.ToString(), Colors.TextWorldGenSetting);
        }

        public static implicit operator T(AbstractWorldGenSetting<T> setting) => setting.Value;
    }
}
