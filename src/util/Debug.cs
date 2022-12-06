using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Xna.Framework;

namespace MinicraftGame.Utils
{
    public static class Debug
    {
        public const float TIME_SCALE_STEP = 0.1f;

        public static bool Enabled = false;
        public static bool DisplayBlockChecks = false;
        public static bool GiveBlocksElseItems = true;
        public static float TimeScale
        {
            get => _timeScale;
            set => _timeScale = MathHelper.Clamp(value, 0f, 5f);
        }

        private static readonly Hashtable _debugUpdates = new Hashtable();
        private static float _timeScale = 1f;

        public static void Tick() => _debugUpdates.Clear();

        public static void AddBlockInteract(Point blockPos) => Add(blockPos, Colors.DebugReason_BlockInteract);

        public static void AddRandomBlockTick(Point blockPos) => Add(blockPos, Colors.DebugReason_RandomBlockTick);

        public static void AddCollisionCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_CollisionCheck);

        public static void AddAirCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_AirCheck);

        public static void AddGrassSpreadCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_GrassSpreadCheck);

        public static void AddWoodCheck(Point blockPos) => Add(blockPos, Colors.DebugReason_WoodCheck);

        public static bool HasDebugUpdate(Point blockPos) => _debugUpdates.Contains(blockPos);

        public static ImmutableArray<Color> GetDebugColors(Point blockPos)
        {
            // get entry list
            var entryList = GetDebugColorEntries(blockPos);
            // return as array
            return entryList.ToImmutableArray();
        }

        private static List<Color> GetDebugColorEntries(Point blockPos) => (List<Color>)_debugUpdates[blockPos];

        private static void Add(Point blockPos, Color color)
        {
            if (!Enabled || !DisplayBlockChecks)
                return;
            // if entry does not exist for point
            if (!HasDebugUpdate(blockPos))
            {
                // create new list for point
                var entries = new List<Color>(new[] { color });
                // add list to hash table
                _debugUpdates.Add(blockPos, entries);
            }
            // entry already exists for point
            else
            {
                // get list for point
                var entries = GetDebugColorEntries(blockPos);
                // add color to list
                entries.Add(color);
            }
        }
    }
}
