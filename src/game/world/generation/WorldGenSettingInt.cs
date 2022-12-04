using Microsoft.Xna.Framework;

namespace MinicraftGame.Game.Worlds.Generation
{
    public class WorldGenSettingInt : AbstractWorldGenSetting<int>
    {
        public WorldGenSettingInt(Vector2 relativeScreenPosition, string name, int defaultValue, int min, int max, int step, int stepShift) : base(relativeScreenPosition, name, defaultValue, min, max, step, stepShift) {}

        public override void IncrementFunc() => Value += StepValue;

        public override void DecrementFunc() => Value -= StepValue;
    }
}
