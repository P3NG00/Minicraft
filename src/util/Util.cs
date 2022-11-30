using System;
using System.IO;
using Microsoft.Xna.Framework;
using MinicraftGame.Game.BlockType;
using MinicraftGame.Game.Inventories;
using MinicraftGame.Game.ItemType;

namespace MinicraftGame.Utils
{
    public static class Util
    {
        // TODO implement 'renderer' classes like InventoryRenderer that take data from Inventory and render it to the screen

        public const int UI_SPACER = 5;

        public static readonly Random Random = new Random();

        public static bool IsInteger(this float f) => f % 1f == 0f;

        public static int Floor(this float f) => (int)Math.Floor((double)f);

        public static T GetRandom<T>(this T[] t) => t[Random.Next(t.Length)];

        public static bool TestChance(this float chance)
        {
            if (chance >= 1.0f)
                return true;
            if (chance < 0.0f)
                return false;
            return Random.NextDouble() < chance;
        }

        public static bool TestChance(this decimal chance)
        {
            if (chance >= 1.0m)
                return true;
            if (chance < 0.0m)
                return false;
            return (decimal)Random.NextDouble() < chance;
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
            var id = stream.ReadByte();
            switch (id)
            {
                case 0: return Items.FromID(stream.ReadInt32());
                case 1: return new BlockItem(Blocks.FromID(stream.ReadInt32()));
            }
            return null;
        }

        public static Block ReadBlock(this BinaryReader stream) => Blocks.FromID(stream.ReadInt32());

        public static void Write(this BinaryWriter stream, Item item)
        {
            if (item is BlockItem blockItem)
            {
                stream.Write((byte)1);
                stream.Write(blockItem.Block);
            }
            else
            {
                stream.Write((byte)0);
                stream.Write(item.ID);
            }
        }

        public static void Write(this BinaryWriter stream, Block block) => stream.Write(block.ID);

        public delegate void ActionRef<T>(ref T t);
    }
}
