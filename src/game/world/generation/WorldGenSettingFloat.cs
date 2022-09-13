using Microsoft.Xna.Framework;

namespace Minicraft.Game.Worlds.Generation
{
    public sealed class WorldGenSettingFloat : AbstractWorldGenSetting<float>
    {
        public WorldGenSettingFloat(Vector2 relativeScreenPosition, string name, float defaultValue, float min, float max, float step) : base(relativeScreenPosition, name, defaultValue, min, max, step) {}

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