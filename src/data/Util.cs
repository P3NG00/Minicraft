using System;
using Microsoft.Xna.Framework;

namespace Game.Data
{
    public static class Util
    {
        public static readonly Random Random = new Random();

        public static readonly Point UpPoint = new Point(0, 1);

        public static T GetRandom<T>(this T[] t) => t[Random.Next(t.Length)];
    }
}
