using System;
using Microsoft.Xna.Framework;
using Minicraft.Game.Blocks;

namespace Minicraft.Utils
{
    public static class Util
    {
        public static readonly Random Random = new Random();

        public static int Clamp(this int i, int min, int max) => Math.Max(Math.Min(i, max), min);

        public static float Clamp(this float f, float min, float max) => Math.Max(Math.Min(f, max), min);

        public static bool IsInteger(this float f) => f % 1f == 0f;

        public static T GetRandom<T>(this T[] t) => t[Random.Next(t.Length)];

        public static Block GetBlock(this BlockType blockType) => (Block)blockType;

        public static bool TestChance(this float chance)
        {
            if (chance >= 1.0f)
                return true;
            if (chance < 0.0f)
                return false;
            return Random.NextDouble() < chance;
        }

        public static bool NextBool(this Random random) => (0.5f).TestChance();

        public static Point NextPoint(this Random random, Point max) => new Point(random.Next(max.X), random.Next(max.Y));

        public delegate void ActionRef<T>(ref T t);
    }
}
