using Microsoft.Xna.Framework;

namespace MinicraftGame.Game.Worlds.Generation
{
    public sealed class WorldGenSettingIntMult : WorldGenSettingInt
    {
        public WorldGenSettingIntMult(Vector2 relativeScreenPosition, string name, int defaultValue, int min, int max, int step, int stepShift) : base(relativeScreenPosition, name, defaultValue, min, max, step, stepShift) {}

        protected sealed override void IncrementFunc()
        {
            for (int i = 0; i < StepValue; i++)
                Value *= 2;
        }

        protected sealed override void DecrementFunc()
        {
            for (int i = 0; i < StepValue; i++)
                Value /= 2;
        }
    }
}
