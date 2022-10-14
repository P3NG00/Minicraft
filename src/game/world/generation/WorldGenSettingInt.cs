using Microsoft.Xna.Framework;

namespace MinicraftGame.Game.Worlds.Generation
{
    public class WorldGenSettingInt : AbstractWorldGenSetting<int>
    {
        public WorldGenSettingInt(Vector2 relativeScreenPosition, string name, int defaultValue, int min, int max, int step, int stepShift) : base(relativeScreenPosition, name, defaultValue, min, max, step, stepShift) {}

        public override void Increment()
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

        public override void Decrement()
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
    }
}
