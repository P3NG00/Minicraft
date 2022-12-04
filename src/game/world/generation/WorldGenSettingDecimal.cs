using Microsoft.Xna.Framework;

namespace MinicraftGame.Game.Worlds.Generation
{
    public sealed class WorldGenSettingDecimal : AbstractWorldGenSetting<decimal>
    {
        public WorldGenSettingDecimal(Vector2 relativeScreenPosition, string name, decimal defaultValue, decimal min, decimal max, decimal step, decimal stepShift) : base(relativeScreenPosition, name, defaultValue, min, max, step, stepShift) {}

        public sealed override void IncrementFunc() => Value += StepValue;

        public sealed override void DecrementFunc() => Value -= StepValue;

        public static explicit operator float(WorldGenSettingDecimal setting) => (float)setting.Value;
    }
}
