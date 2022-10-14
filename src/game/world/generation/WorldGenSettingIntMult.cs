using Microsoft.Xna.Framework;

namespace MinicraftGame.Game.Worlds.Generation
{
    public sealed class WorldGenSettingIntMult : WorldGenSettingInt
    {
        public WorldGenSettingIntMult(Vector2 relativeScreenPosition, string name, int defaultValue, int min, int max, int step, int stepShift) : base(relativeScreenPosition, name, defaultValue, min, max, step, stepShift) {}

        public sealed override void Increment()
        {
            // increment value if below max
            if (Value < Max)
            {
                for (int i = 0; i < StepValue; i++)
                    Value *= 2;
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
                for (int i = 0; i < StepValue; i++)
                    Value /= 2;
                // ensure value at least min
                if (Value < Min)
                    Value = Min;
            }
        }
    }
}
