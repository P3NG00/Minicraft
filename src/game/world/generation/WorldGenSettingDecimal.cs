using Microsoft.Xna.Framework;

namespace Minicraft.Game.Worlds.Generation
{
    public sealed class WorldGenSettingDecimal : AbstractWorldGenSetting<decimal>
    {
        public WorldGenSettingDecimal(Vector2 relativeScreenPosition, string name, decimal defaultValue, decimal min, decimal max, decimal step, decimal stepShift) : base(relativeScreenPosition, name, defaultValue, min, max, step, stepShift) {}

        public sealed override void Increment()
        {
            // increment value if below max
            if (Value < Max)
            {
                Value += StepValue;
                // ensure value at most max
                if (Value > Max)
                    Value = Max;
            }
        }

        public sealed override void Decrement()
        {
            // decrement value if above min
            if (Value > Min)
            {
                Value -= StepValue;
                // ensure value at least min
                if (Value < Min)
                    Value = Min;
            }
        }

        public static explicit operator float(WorldGenSettingDecimal setting) => (float)setting.Value;
    }
}