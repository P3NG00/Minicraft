using Microsoft.Xna.Framework;

namespace Minicraft.Game.Worlds.Generation
{
    public sealed class WorldGenSettingInt : AbstractWorldGenSetting<int>
    {
        public WorldGenSettingInt(Vector2 relativeScreenPosition, string name, int defaultValue, int min, int max, int step) : base(relativeScreenPosition, name, defaultValue, min, max, step) {}

        public sealed override void Increment()
        {
            if (Value < Max)
                Value += Step;
        }

        public sealed override void Decrement()
        {
            if (Value > Min)
                Value -= Step;
        }
    }
}
