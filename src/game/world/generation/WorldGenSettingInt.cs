using Microsoft.Xna.Framework;

namespace MinicraftGame.Game.Worlds.Generation
{
    public class WorldGenSettingInt : AbstractWorldGenSetting<int>
    {
        public WorldGenSettingInt(Vector2 relativeScreenPosition, string name, int defaultValue, int min, int max, int step, int stepShift) : base(relativeScreenPosition, name, defaultValue, min, max, step, stepShift) {}

        protected override void IncrementFunc() => Value += StepValue;

        protected override void DecrementFunc() => Value -= StepValue;
    }
}
