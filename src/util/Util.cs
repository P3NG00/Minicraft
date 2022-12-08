using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.BlockType;
using MinicraftGame.Game.ItemType;

namespace MinicraftGame.Utils
{
    public static class Util
    {
        public const int UI_SPACER = 5;

        public static readonly Random Random = new Random();

        public static void SingletonCheck<T>(ref T singleton, T instance)
        {
            if (singleton != null)
                throw new System.Exception("Singleton already instantiated.");
            singleton = instance;
        }

        public static void Toggle(ref bool b) => b = !b;

        public static void ForEach<T>(this T[] array, Action<T> action) => Array.ForEach(array, action);

        public static bool IsInteger(this float f) => f % 1f == 0f;

        public static int Floor(this float f) => (int)Math.Floor((double)f);

        public static T GetRandom<T>(this T[] t) => t[Random.Next(t.Length)];

        public static bool TestChance(this float chance)
        {
            if (chance >= 1.0f)
                return true;
            if (chance <= 0.0f)
                return false;
            return Random.NextDouble() < chance;
        }

        public static bool TestChance(this decimal chance)
        {
            if (chance >= 1.0m)
                return true;
            if (chance <= 0.0m)
                return false;
            return (decimal)Random.NextDouble() < chance;
        }

        public static double Average(this IEnumerable<ulong> source)
        {
            ulong sum = 0;
            ulong count = 0;
            foreach (ulong i in source)
            {
                sum += i;
                count++;
            }
            return (double)sum / (double)count;
        }

        public static bool NextBool(this Random random) => (0.5f).TestChance();

        public static Point NextPoint(this Random random, Point max) => new Point(random.Next(max.X), random.Next(max.Y));

        public static Vector2 NextUnitVector(this Random random)
        {
            float angle = (float)random.NextDouble() * MathHelper.TwoPi;
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static Item ReadItem(this BinaryReader stream)
        {
            int itemTypeID = stream.ReadByte();
            int id = stream.ReadInt32();
            switch (itemTypeID)
            {
                case (byte)ItemType.Item: return Items.FromID(id);
                case (byte)ItemType.BlockItem: return new BlockItem(Blocks.FromID(id));
                default: throw new Exception("Invalid item type ID: " + itemTypeID);
            }
        }

        public static Block ReadBlock(this BinaryReader stream) => Blocks.FromID(stream.ReadInt32());

        public static void Write(this BinaryWriter stream, Item item)
        {
            if (item is BlockItem blockItem)
            {
                stream.Write((byte)ItemType.BlockItem);
                stream.Write(blockItem.Block);
            }
            else
            {
                stream.Write((byte)ItemType.Item);
                stream.Write(item.ID);
            }
        }

        public static void Write(this BinaryWriter stream, Block block) => stream.Write(block.ID);

        public delegate void ActionRef<T>(ref T t);

        private enum ItemType
        {
            Item = 0,
            BlockItem = 1,
        }
    }
}
