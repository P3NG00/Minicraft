using System;
using Microsoft.Xna.Framework;

namespace Minicraft.Utils
{
    public static class Util
    {
        public static readonly Random Random = new Random();

        public static T GetRandom<T>(this T[] t) => t[Random.Next(t.Length)];

        public static bool TestChance(this Random random, float chance)
        {
            if (chance >= 1.0f)
                return true;
            else if (chance < 0.0f)
                return false;
            else
                return random.NextDouble() < chance;
        }

        public static bool NextBool(this Random random) => random.NextDouble() < 0.5;

        public static Point NextPoint(this Random random, Point max) => new Point(random.Next(max.X), random.Next(max.Y));
    }
}
